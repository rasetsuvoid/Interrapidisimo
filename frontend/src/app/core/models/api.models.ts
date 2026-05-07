export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
  errors: string[];
  traceId?: string;
  timestamp: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface Student {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  studentCode: string;
  phoneNumber?: string;
  dateOfBirth: string;
  enrolledSubjectsCount: number;
  totalCredits: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateStudent {
  firstName: string;
  lastName: string;
  email: string;
  studentCode: string;
  dateOfBirth: string;
  phoneNumber?: string;
}

export interface UpdateStudent {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  dateOfBirth: string;
}

export interface RegisterStudent {
  firstName: string;
  lastName: string;
  email: string;
  studentCode: string;
  dateOfBirth: string;
  phoneNumber?: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  student: Student;
}

export interface Subject {
  id: string;
  name: string;
  code: string;
  description?: string;
  credits: number;
  professorId: string;
  professorName: string;
  professorEmail: string;
  enrolledStudentsCount: number;
}

export interface SubjectSummary {
  id: string;
  name: string;
  code: string;
  credits: number;
  professorId: string;
  professorName: string;
}

export interface Professor {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  bio?: string;
  subjects: SubjectSummary[];
}

export interface EnrollmentDto {
  id: string;
  studentId: string;
  studentName: string;
  subjectId: string;
  subjectName: string;
  subjectCode: string;
  professorName: string;
  credits: number;
  enrolledAt: string;
}

export interface Classmate {
  studentId: string;
  fullName: string;
}

export interface SubjectEnrollmentSummary {
  subjectId: string;
  subjectName: string;
  subjectCode: string;
  professorName: string;
  credits: number;
  enrolledAt: string;
  classmates: Classmate[];
}

export interface StudentEnrollmentStatus {
  studentId: string;
  studentName: string;
  enrolledSubjectsCount: number;
  maxSubjects: number;
  totalCredits: number;
  enrolledSubjects: SubjectSummary[];
  availableSubjects: SubjectSummary[];
}

export interface EnrollRequest {
  studentId: string;
  subjectId: string;
}

export interface WithdrawRequest {
  studentId: string;
  subjectId: string;
}

export interface Toast {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  message: string;
}
