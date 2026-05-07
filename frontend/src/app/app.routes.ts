import { Routes } from '@angular/router';
import { LayoutComponent } from './shared/components/ui/layout/layout.component';
import { authChildGuard, guestGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: 'auth',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/auth.component').then(m => m.AuthComponent)
  },
  {
    path: '',
    component: LayoutComponent,
    canActivateChild: [authChildGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'students',
        loadComponent: () => import('./features/students/students.component').then(m => m.StudentsComponent)
      },
      {
        path: 'students/:id',
        loadComponent: () => import('./features/students/student-detail/student-detail.component').then(m => m.StudentDetailComponent)
      },
      {
        path: 'subjects',
        loadComponent: () => import('./features/subjects/subjects.component').then(m => m.SubjectsComponent)
      }
    ]
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];
