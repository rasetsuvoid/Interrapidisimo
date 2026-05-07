import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, from, switchMap, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, AuthResponse, LoginRequest, RegisterStudent, Student } from '../models/api.models';
import { AuthSessionService, StoredSession } from './auth-session.service';
import { CryptoService } from './crypto.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly session = inject(AuthSessionService);
  private readonly crypto = inject(CryptoService);
  private readonly base = environment.apiUrl;

  register(dto: RegisterStudent): Observable<ApiResponse<AuthResponse>> {
    return from(this.crypto.encrypt(dto.password)).pipe(
      switchMap(encryptedPassword =>
        this.http.post<ApiResponse<AuthResponse>>(`${this.base}/auth/register`, {
          firstName: dto.firstName,
          lastName: dto.lastName,
          email: dto.email,
          studentCode: dto.studentCode,
          dateOfBirth: dto.dateOfBirth,
          phoneNumber: dto.phoneNumber,
          encryptedPassword
        })
      ),
      tap(response => this.persistFromResponse(response, true))
    );
  }

  login(dto: LoginRequest, rememberSession = true): Observable<ApiResponse<AuthResponse>> {
    return from(this.crypto.encrypt(dto.password)).pipe(
      switchMap(encryptedPassword =>
        this.http.post<ApiResponse<AuthResponse>>(`${this.base}/auth/login`, {
          email: dto.email,
          encryptedPassword
        })
      ),
      tap(response => this.persistFromResponse(response, rememberSession))
    );
  }

  logout(navigate = true): void {
    this.session.clear(navigate);
  }

  private persistFromResponse(response: ApiResponse<AuthResponse>, persistent: boolean): void {
    if (!response.data) return;

    const session: StoredSession = {
      token: response.data.token,
      expiresAt: response.data.expiresAt,
      student: response.data.student
    };

    this.session.persist(session, persistent);
  }
}
