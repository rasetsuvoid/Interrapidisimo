import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';
import { LoginRequest, RegisterStudent } from '../../core/models/api.models';

@Component({
  selector: 'app-auth',
  imports: [FormsModule],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.scss'
})
export class AuthComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  mode = signal<'login' | 'register'>('login');
  submitting = signal(false);
  showLoginPassword = signal(false);
  showRegisterPassword = signal(false);
  serverError = signal<string | null>(null);
  private errorTimer: ReturnType<typeof setTimeout> | null = null;

  private showError(msg: string, durationMs = 5000): void {
    if (this.errorTimer) clearTimeout(this.errorTimer);
    this.serverError.set(msg);
    this.errorTimer = setTimeout(() => this.serverError.set(null), durationMs);
  }
  rememberSession = true;

  loginData: LoginRequest = {
    email: '',
    password: ''
  };

  registerData: RegisterStudent = {
    firstName: '',
    lastName: '',
    email: '',
    studentCode: '',
    dateOfBirth: '',
    phoneNumber: '',
    password: ''
  };

  setMode(mode: 'login' | 'register'): void {
    this.mode.set(mode);
    this.serverError.set(null);
  }

  submitLogin(): void {
    this.serverError.set(null);
    this.submitting.set(true);
    this.auth.login(this.loginData, this.rememberSession).subscribe({
      next: () => {
        this.toast.success('Inicio de sesión correcto.');
        void this.router.navigate(['/dashboard']);
        this.submitting.set(false);
      },
      error: err => {
        const msg = err.error?.errors?.[0] ?? err.error?.message ?? 'No se pudo iniciar sesión.';
        this.showError(msg);
        this.submitting.set(false);
      }
    });
  }

  submitRegister(): void {
    this.serverError.set(null);
    this.submitting.set(true);
    this.auth.register(this.registerData).subscribe({
      next: () => {
        this.toast.success('Registro completado. Bienvenido.');
        void this.router.navigate(['/dashboard']);
        this.submitting.set(false);
      },
      error: err => {
        const msg = err.error?.errors?.[0] ?? err.error?.message ?? 'No se pudo completar el registro.';
        this.showError(msg);
        this.submitting.set(false);
      }
    });
  }

  toggleLoginPasswordVisibility(): void {
    this.showLoginPassword.update(value => !value);
  }

  toggleRegisterPasswordVisibility(): void {
    this.showRegisterPassword.update(value => !value);
  }

  openDatePicker(input: HTMLInputElement): void {
    const pickerInput = input as HTMLInputElement & { showPicker?: () => void };

    if (typeof pickerInput.showPicker === 'function') {
      pickerInput.showPicker();
      return;
    }

    pickerInput.focus();
    pickerInput.click();
  }
}
