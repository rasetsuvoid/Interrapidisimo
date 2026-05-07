import { Component, computed, inject, input, output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { AuthSessionService } from '../../../../core/services/auth-session.service';

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  protected readonly session = inject(AuthSessionService);
  protected readonly auth = inject(AuthService);
  readonly isOpen = input(false);
  readonly closeRequest = output<void>();

  protected readonly misMateriasRoute = computed(() => {
    const student = this.session.currentStudent();
    return student ? `/students/${student.id}` : '/dashboard';
  });

  close(): void { this.closeRequest.emit(); }

  logout(): void {
    this.auth.logout();
  }
}
