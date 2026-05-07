import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { lastValueFrom } from 'rxjs';
import { ApiService } from '../../core/services/api.service';
import { AuthSessionService } from '../../core/services/auth-session.service';
import { Subject, Professor, StudentEnrollmentStatus } from '../../core/models/api.models';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private readonly api = inject(ApiService);
  protected readonly session = inject(AuthSessionService);

  loading = signal(true);
  subjects = signal<Subject[]>([]);
  professors = signal<Professor[]>([]);
  enrollmentStatus = signal<StudentEnrollmentStatus | null>(null);

  totalCredits = computed(() =>
    this.subjects().reduce((sum, subject) => sum + subject.credits, 0)
  );
  averageSubjectsPerProfessor = computed(() => {
    const professorCount = this.professors().length;
    const subjectCount = this.subjects().length;
    return professorCount ? (subjectCount / professorCount).toFixed(1) : '0';
  });
  misMateriasRoute = computed(() => {
    const student = this.session.currentStudent();
    return student ? `/students/${student.id}` : '/dashboard';
  });

  ngOnInit(): void {
    const studentId = this.session.currentStudent()?.id;
    const requests: Promise<unknown>[] = [
      lastValueFrom(this.api.getSubjects()),
      lastValueFrom(this.api.getProfessors())
    ];
    if (studentId) {
      requests.push(lastValueFrom(this.api.getEnrollmentStatus(studentId)));
    }

    Promise.all(requests).then(([sub, p, st]) => {
      this.subjects.set((sub as any)?.data ?? []);
      this.professors.set((p as any)?.data ?? []);
      if (st) this.enrollmentStatus.set((st as any)?.data ?? null);
      this.loading.set(false);
    }).catch(() => this.loading.set(false));
  }

  creditProgressPercent = computed(() => {
    const credits = this.enrollmentStatus()?.totalCredits ?? 0;
    return Math.round((credits / 9) * 100);
  });

  openSlots = computed(() => {
    const status = this.enrollmentStatus();
    if (!status) return 3;
    return status.maxSubjects - status.enrolledSubjectsCount;
  });

  subjectInitials(name: string): string {
    return name
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map(part => part[0]?.toUpperCase() ?? '')
      .join('');
  }

  enrolledHeaderClass(index: number): string {
    return ['hdr-primary', 'hdr-secondary', 'hdr-tertiary'][index % 3];
  }

  enrolledBadgeClass(index: number): string {
    return ['badge-primary', 'badge-secondary', 'badge-tertiary'][index % 3];
  }
}
