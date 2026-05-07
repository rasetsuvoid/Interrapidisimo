import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ToastContainerComponent } from './shared/components/ui/toast-container/toast-container.component';
import { RateLimitModalComponent } from './shared/components/ui/rate-limit-modal/rate-limit-modal.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ToastContainerComponent, RateLimitModalComponent],
  template: `
    <div class="min-h-screen bg-base-100">
      <router-outlet />
      <app-toast-container />
      <app-rate-limit-modal />
    </div>
  `
})
export class App {}
