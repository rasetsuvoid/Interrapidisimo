import { Component, inject } from '@angular/core';
import { RateLimitService } from '../../../../core/services/rate-limit.service';

@Component({
  selector: 'app-rate-limit-modal',
  templateUrl: './rate-limit-modal.component.html',
  styleUrl:    './rate-limit-modal.component.scss',
})
export class RateLimitModalComponent {
  protected readonly rl = inject(RateLimitService);
}
