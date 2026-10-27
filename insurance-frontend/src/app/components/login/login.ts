
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RouterLink } from '@angular/router';
import { InsuranceService } from '../../services/insurance';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  private insuranceService = inject(InsuranceService);
  private router = inject(Router);

  email = '';
  password = '';
  errorMessage = '';
  loading = false;

  onLogin() {
    this.errorMessage = '';
    this.loading = true;

    this.insuranceService.login({
      email: this.email,
      password: this.password
    }).subscribe({

      next: (response) => {
        sessionStorage.setItem('token', response.token);
        this.loading = false;

        const payload = JSON.parse(atob(response.token.split('.')[1]));

        const role =
          payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
          ?? payload.role;

        if (role === 'Admin') {
          this.router.navigate(['/admin']);
        } else {
          this.router.navigate(['/products']);
        }
      },

      error: (error) => {
        console.error(error);
        this.errorMessage = 'Invalid email or password.';
        this.loading = false;
      }
    });
  }
}
