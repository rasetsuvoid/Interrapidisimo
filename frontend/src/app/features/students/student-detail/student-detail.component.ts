import { NgClass } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { lastValueFrom } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { AuthSessionService } from '../../../core/services/auth-session.service';
import { Classmate, Student, StudentEnrollmentStatus, SubjectEnrollmentSummary } from '../../../core/models/api.models';

const SUBJECT_STYLES = [
  { icon: 'functions',    bg: 'bg-primary/10',   color: 'text-primary'   },
  { icon: 'account_tree', bg: 'bg-secondary/10', color: 'text-secondary' },
  { icon: 'database',     bg: 'bg-accent/10',    color: 'text-accent'    },
  { icon: 'science',      bg: 'bg-primary/10',   color: 'text-primary'   },
  { icon: 'code',         bg: 'bg-secondary/10', color: 'text-secondary' },
  { icon: 'calculate',    bg: 'bg-accent/10',    color: 'text-accent'    },
];

@Component({
  selector: 'app-student-detail',
  imports: [RouterLink, NgClass],
  templateUrl: './student-detail.component.html',
  styleUrl: './student-detail.component.scss'
})
export class StudentDetailComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly session = inject(AuthSessionService);

  readonly isOwnProfile = computed(() => {
    const id = this.route.snapshot.paramMap.get('id');
    return id === this.session.currentStudent()?.id;
  });

  loading = signal(true);
  student = signal<Student | null>(null);
  status = signal<StudentEnrollmentStatus | null>(null);
  enrollments = signal<SubjectEnrollmentSummary[]>([]);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.loadAll(id);
  }

  private loadAll(id: string): void {
    this.loading.set(true);
    Promise.all([
      lastValueFrom(this.api.getStudent(id)),
      lastValueFrom(this.api.getEnrollmentStatus(id)),
      lastValueFrom(this.api.getClassmates(id))
    ]).then(([s, st, cl]) => {
      this.student.set(s?.data ?? null);
      this.status.set(st?.data ?? null);
      this.enrollments.set(cl?.data ?? []);
      this.loading.set(false);
    }).catch(() => this.loading.set(false));
  }

  initials(): string {
    const s = this.student();
    return s ? (s.firstName[0] + s.lastName[0]).toUpperCase() : '';
  }

  creditProgressPercent(): number {
    return Math.round(((this.status()?.totalCredits ?? 0) / 9) * 100);
  }

  subjectIcon(index: number): string {
    return SUBJECT_STYLES[index % SUBJECT_STYLES.length].icon;
  }

  subjectIconClass(index: number): string {
    return ['sd-icon-primary', 'sd-icon-secondary', 'sd-icon-tertiary'][index % 3];
  }

  subjectCardAccent(index: number): string {
    return ['', 'accent-secondary', 'accent-tertiary'][index % 3];
  }

  subjectIconBg(index: number): string {
    return SUBJECT_STYLES[index % SUBJECT_STYLES.length].bg;
  }

  subjectIconColor(index: number): string {
    return SUBJECT_STYLES[index % SUBJECT_STYLES.length].color;
  }

  classmateInitials(name: string): string {
    const parts = name.trim().split(' ');
    return parts.length >= 2
      ? (parts[0][0] + parts.at(-1)?.[0]).toUpperCase()
      : name.slice(0, 2).toUpperCase();
  }

  visibleClassmates(classmates: Classmate[]): Classmate[] {
    return classmates.slice(0, 2);
  }

  extraClassmates(classmates: Classmate[]): number {
    return Math.max(0, classmates.length - 2);
  }
}
