import { NgClass } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { ApiService } from '../../core/services/api.service';
import { AuthSessionService } from '../../core/services/auth-session.service';
import { ToastService } from '../../core/services/toast.service';
import { Subject, StudentEnrollmentStatus } from '../../core/models/api.models';

@Component({
  selector: 'app-subjects',
  imports: [NgClass],
  templateUrl: './subjects.component.html',
  styleUrl: './subjects.component.scss'
})
export class SubjectsComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly session = inject(AuthSessionService);
  private readonly toast = inject(ToastService);

  loading = signal(true);
  subjects = signal<Subject[]>([]);
  enrollmentStatus = signal<StudentEnrollmentStatus | null>(null);
  enrollingId = signal<string | null>(null);
  withdrawingId = signal<string | null>(null);

  totalCredits = computed(() =>
    this.subjects().reduce((sum, s) => sum + s.credits, 0)
  );
  remainingSlots = computed(() => {
    const st = this.enrollmentStatus();
    if (!st) return 0;
    return Math.max(0, st.maxSubjects - st.enrolledSubjectsCount);
  });
  totalEnrollments = computed(() =>
    this.subjects().reduce((sum, s) => sum + s.enrolledStudentsCount, 0)
  );
  averageEnrollments = computed(() => {
    const list = this.subjects();
    if (!list.length) return '0.0';
    return (this.totalEnrollments() / list.length).toFixed(1);
  });

  private readonly enrolledIds = computed(() =>
    new Set(this.enrollmentStatus()?.enrolledSubjects.map(s => s.id) ?? [])
  );
  private readonly availableIds = computed(() =>
    new Set(this.enrollmentStatus()?.availableSubjects.map(s => s.id) ?? [])
  );
  isFull = computed(() => {
    const st = this.enrollmentStatus();
    return st ? st.enrolledSubjectsCount >= st.maxSubjects : false;
  });

  subjectState(subjectId: string): 'enrolled' | 'available' | 'conflict' | 'full' | 'guest' {
    if (!this.enrollmentStatus()) return 'guest';
    if (this.enrolledIds().has(subjectId)) return 'enrolled';
    if (this.isFull()) return 'full';
    if (this.availableIds().has(subjectId)) return 'available';
    return 'conflict';
  }

  ngOnInit(): void {
    const studentId = this.session.currentStudent()?.id;
    const requests: Promise<unknown>[] = [
      lastValueFrom(this.api.getSubjects())
    ];
    if (studentId) {
      requests.push(lastValueFrom(this.api.getEnrollmentStatus(studentId)));
    }

    Promise.all(requests).then(([sub, st]) => {
      this.subjects.set((sub as any)?.data ?? []);
      if (st) this.enrollmentStatus.set((st as any)?.data ?? null);
      this.loading.set(false);
    }).catch(() => this.loading.set(false));
  }

  enroll(subject: Subject): void {
    const studentId = this.session.currentStudent()?.id;
    if (!studentId) return;
    this.enrollingId.set(subject.id);
    this.api.enroll({ studentId, subjectId: subject.id }).subscribe({
      next: () => {
        this.toast.success(`Inscripción realizada en ${subject.name}`);
        this.reloadStatus(studentId);
        this.reloadSubjects();
        this.enrollingId.set(null);
      },
      error: err => {
        this.toast.error(err.error?.errors?.[0] ?? 'No se pudo realizar la inscripción.');
        this.enrollingId.set(null);
      }
    });
  }

  withdraw(subject: Subject): void {
    const studentId = this.session.currentStudent()?.id;
    if (!studentId) return;
    this.withdrawingId.set(subject.id);
    this.api.withdraw({ studentId, subjectId: subject.id }).subscribe({
      next: () => {
        this.toast.success(`Retiro de ${subject.name} realizado.`);
        this.reloadStatus(studentId);
        this.reloadSubjects();
        this.withdrawingId.set(null);
      },
      error: () => {
        this.toast.error('No se pudo retirar la materia.');
        this.withdrawingId.set(null);
      }
    });
  }

  private reloadStatus(studentId: string): void {
    this.api.getEnrollmentStatus(studentId).subscribe(res => {
      this.enrollmentStatus.set(res.data ?? null);
    });
  }

  private reloadSubjects(): void {
    this.api.getSubjects().subscribe(res => {
      this.subjects.set(res.data ?? []);
    });
  }

  avatarClass(index: number): string {
    return ['av-primary', 'av-secondary', 'av-tertiary', 'av-neutral'][index % 4];
  }

  professorInitials(name: string): string {
    return name
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map(part => part[0]?.toUpperCase() ?? '')
      .join('');
  }
}
