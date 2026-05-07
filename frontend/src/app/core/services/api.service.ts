import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ApiResponse, PagedResult, Student, CreateStudent, UpdateStudent,
  Subject, Professor, EnrollmentDto, StudentEnrollmentStatus,
  SubjectEnrollmentSummary, EnrollRequest, WithdrawRequest
} from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiUrl;

  // Students
  getStudents(page = 1, pageSize = 10, search?: string): Observable<ApiResponse<PagedResult<Student>>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (search) params = params.set('search', search);
    return this.http.get<ApiResponse<PagedResult<Student>>>(`${this.base}/students`, { params });
  }

  getStudent(id: string): Observable<ApiResponse<Student>> {
    return this.http.get<ApiResponse<Student>>(`${this.base}/students/${id}`);
  }

  createStudent(dto: CreateStudent): Observable<ApiResponse<Student>> {
    return this.http.post<ApiResponse<Student>>(`${this.base}/students`, dto);
  }

  updateStudent(id: string, dto: UpdateStudent): Observable<ApiResponse<Student>> {
    return this.http.put<ApiResponse<Student>>(`${this.base}/students/${id}`, dto);
  }

  deleteStudent(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/students/${id}`);
  }

  // Subjects
  getSubjects(): Observable<ApiResponse<Subject[]>> {
    return this.http.get<ApiResponse<Subject[]>>(`${this.base}/subjects`);
  }

  getSubject(id: string): Observable<ApiResponse<Subject>> {
    return this.http.get<ApiResponse<Subject>>(`${this.base}/subjects/${id}`);
  }

  // Professors
  getProfessors(): Observable<ApiResponse<Professor[]>> {
    return this.http.get<ApiResponse<Professor[]>>(`${this.base}/professors`);
  }

  // Enrollments
  enroll(dto: EnrollRequest): Observable<ApiResponse<EnrollmentDto>> {
    return this.http.post<ApiResponse<EnrollmentDto>>(`${this.base}/enrollments`, dto);
  }

  withdraw(dto: WithdrawRequest): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.base}/enrollments`, { body: dto });
  }

  getEnrollmentStatus(studentId: string): Observable<ApiResponse<StudentEnrollmentStatus>> {
    return this.http.get<ApiResponse<StudentEnrollmentStatus>>(`${this.base}/enrollments/student/${studentId}/status`);
  }

  getClassmates(studentId: string): Observable<ApiResponse<SubjectEnrollmentSummary[]>> {
    return this.http.get<ApiResponse<SubjectEnrollmentSummary[]>>(`${this.base}/enrollments/student/${studentId}/classmates`);
  }
}
