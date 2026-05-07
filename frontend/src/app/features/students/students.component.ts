import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { ToastService } from '../../core/services/toast.service';
import { SkeletonComponent } from '../../shared/components/ui/skeleton/skeleton.component';
import { Student, UpdateStudent } from '../../core/models/api.models';

@Component({
  selector: 'app-students',
  imports: [RouterLink, FormsModule, SkeletonComponent],
  templateUrl: './students.component.html',
  styleUrl: './students.component.scss'
})
export class StudentsComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly toast = inject(ToastService);

  loading = signal(true);
  saving = signal(false);
  showModal = signal(false);
  showDeleteConfirm = signal(false);
  students = signal<Student[]>([]);
  editingStudent = signal<Student | null>(null);
  deletingStudent = signal<Student | null>(null);
  currentPage = signal(1);
  totalPages = signal(1);
  searchTerm = '';
  private searchTimeout: ReturnType<typeof setTimeout> | null = null;

  displayedStudents = computed(() => this.students().length);
  averageCredits = computed(() => {
    const list = this.students();
    if (!list.length) return '0.0';
    const total = list.reduce((sum, student) => sum + student.totalCredits, 0);
    return (total / list.length).toFixed(1);
  });
  fullyLoadedStudents = computed(() =>
    this.students().filter(student => student.totalCredits === 9).length
  );

  formData = { firstName: '', lastName: '', email: '', dateOfBirth: '', phoneNumber: '' };

  ngOnInit(): void {
    this.loadStudents();
  }

  loadStudents(): void {
    this.loading.set(true);
    this.api.getStudents(this.currentPage(), 10, this.searchTerm || undefined).subscribe({
      next: res => {
        this.students.set(res.data?.items ?? []);
        this.totalPages.set(res.data?.totalPages ?? 1);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  onSearch(): void {
    if (this.searchTimeout) clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => {
      this.currentPage.set(1);
      this.loadStudents();
    }, 400);
  }

  changePage(page: number): void {
    this.currentPage.set(page);
    this.loadStudents();
  }

  openEdit(student: Student): void {
    this.editingStudent.set(student);
    this.formData = {
      firstName: student.firstName,
      lastName: student.lastName,
      email: student.email,
      dateOfBirth: student.dateOfBirth.split('T')[0],
      phoneNumber: student.phoneNumber ?? ''
    };
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  saveStudent(): void {
    const editing = this.editingStudent();
    if (!editing) return;

    this.saving.set(true);
    const dto: UpdateStudent = { ...this.formData };

    this.api.updateStudent(editing.id, dto).subscribe({
      next: () => {
        this.toast.success('Estudiante actualizado.');
        this.closeModal();
        this.loadStudents();
        this.saving.set(false);
      },
      error: err => {
        this.toast.error(err.error?.errors?.[0] ?? 'No se pudo actualizar el estudiante.');
        this.saving.set(false);
      }
    });
  }

  confirmDelete(student: Student): void {
    this.deletingStudent.set(student);
    this.showDeleteConfirm.set(true);
  }

  deleteStudent(): void {
    const s = this.deletingStudent();
    if (!s) return;

    this.saving.set(true);
    this.api.deleteStudent(s.id).subscribe({
      next: () => {
        this.toast.success('Estudiante eliminado.');
        this.showDeleteConfirm.set(false);
        this.loadStudents();
        this.saving.set(false);
      },
      error: () => {
        this.toast.error('No se pudo eliminar el estudiante.');
        this.saving.set(false);
      }
    });
  }

  initials(s: Student): string {
    return (s.firstName[0] + s.lastName[0]).toUpperCase();
  }

  subjectDots(count: number): number[] {
    return new Array(count);
  }

  emptyDots(count: number): number[] {
    return new Array(3 - count);
  }

  creditBadge(c: number): string {
    if (c === 9) return 'badge badge-error badge-sm';
    if (c >= 6) return 'badge badge-warning badge-sm';
    return 'badge badge-success badge-sm';
  }

  progressWidth(credits: number): string {
    return `${Math.min((credits / 9) * 100, 100)}%`;
  }
}
