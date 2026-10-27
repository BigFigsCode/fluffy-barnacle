
import {
  Component,
  ChangeDetectorRef,
  inject
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { InsuranceService } from '../../services/insurance';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  private insuranceService = inject(InsuranceService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  firstName = '';
  lastName = '';
  email = '';
  password = '';

  loading = false;
  errorMessage = '';

  register(): void {
    if (this.loading) return;

    this.errorMessage = '';

    if (!this.firstName.trim() || !this.lastName.trim()) {
      this.errorMessage = 'Please enter your first and last name.';
      return;
    }

    if (this.password.length < 8) {
      this.errorMessage = 'Password must be at least 8 characters.';
      return;
    }

    this.loading = true;

    this.insuranceService.register({
      firstName: this.firstName.trim(),
      lastName: this.lastName.trim(),
      email: this.email.trim(),
      password: this.password
    }).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/login'], {
          state: {
            registrationSuccess: true
          }
        });
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error(error);

        this.errorMessage =
          error.status === 409
            ? 'This email is already registered.'
            : 'Unable to create your account. Please try again.';

        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }
}
