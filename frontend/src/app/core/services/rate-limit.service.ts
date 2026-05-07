import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class RateLimitService {
  readonly isOpen  = signal(false);
  readonly message = signal('');

  show(msg: string): void {
    this.message.set(msg);
    this.isOpen.set(true);
  }

  close(): void {
    this.isOpen.set(false);
  }
}
