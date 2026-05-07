import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Student } from '../models/api.models';

const AUTH_STORAGE_KEY = 'interrapidisimo.auth';

export interface StoredSession {
  token: string;
  expiresAt: string;
  student: Student;
}

@Injectable({ providedIn: 'root' })
export class AuthSessionService {
  private readonly router = inject(Router);
  private readonly sessionState = signal<StoredSession | null>(this.readStoredSession());
  private readonly activeSession = computed(() => {
    const session = this.sessionState();
    if (!session || this.isExpired(session.expiresAt)) {
      return null;
    }

    return session;
  });

  readonly token = computed(() => this.activeSession()?.token ?? null);
  readonly currentStudent = computed(() => this.activeSession()?.student ?? null);
  readonly isAuthenticated = computed(() => !!this.token());

  persist(session: StoredSession, persistent = true): void {
    localStorage.removeItem(AUTH_STORAGE_KEY);
    sessionStorage.removeItem(AUTH_STORAGE_KEY);

    const storage = persistent ? localStorage : sessionStorage;
    storage.setItem(AUTH_STORAGE_KEY, JSON.stringify(session));
    this.sessionState.set(session);
  }

  clear(navigate = true): void {
    localStorage.removeItem(AUTH_STORAGE_KEY);
    sessionStorage.removeItem(AUTH_STORAGE_KEY);
    this.sessionState.set(null);

    if (navigate) {
      void this.router.navigate(['/auth']);
    }
  }

  private readStoredSession(): StoredSession | null {
    const raw = localStorage.getItem(AUTH_STORAGE_KEY) ?? sessionStorage.getItem(AUTH_STORAGE_KEY);
    if (!raw) return null;

    try {
      const parsed = JSON.parse(raw) as StoredSession;
      if (!parsed.token || !parsed.expiresAt || !parsed.student) return null;
      if (this.isExpired(parsed.expiresAt)) return null;
      return parsed;
    } catch {
      return null;
    }
  }

  private isExpired(expiresAt: string): boolean {
    return new Date(expiresAt).getTime() <= Date.now();
  }
}
