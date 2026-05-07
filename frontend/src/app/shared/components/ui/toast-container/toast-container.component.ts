import { Component, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { ToastService } from '../../../../core/services/toast.service';
import { Toast } from '../../../../core/models/api.models';

@Component({
  selector: 'app-toast-container',
  imports: [NgClass],
  templateUrl: './toast-container.component.html',
  styleUrl: './toast-container.component.scss'
})
export class ToastContainerComponent {
  protected readonly toastService = inject(ToastService);

  alertClass(toast: Toast): string {
    const map: Record<Toast['type'], string> = {
      success: 'alert-success',
      error: 'alert-error',
      warning: 'alert-warning',
      info: 'alert-info'
    };
    return map[toast.type];
  }
}
