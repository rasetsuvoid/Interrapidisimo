import { Injectable, signal } from '@angular/core';
import { Toast } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class ToastService {
  readonly toasts = signal<Toast[]>([]);

  show(type: Toast['type'], message: string, duration = 4000): void {
    const toast: Toast = { id: crypto.randomUUID(), type, message };
    this.toasts.update(list => [...list, toast]);
    setTimeout(() => this.dismiss(toast.id), duration);
  }

  success(message: string) { this.show('success', message); }
  error(message: string) { this.show('error', message, 6000); }
  warning(message: string) { this.show('warning', message); }
  info(message: string) { this.show('info', message); }

  dismiss(id: string): void {
    this.toasts.update(list => list.filter(t => t.id !== id));
  }
}
