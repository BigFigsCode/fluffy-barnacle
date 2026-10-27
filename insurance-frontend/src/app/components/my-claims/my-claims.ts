
import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { InsuranceService } from '../../services/insurance';

@Component({
  selector: 'app-my-claims',
  imports: [CommonModule, FormsModule],
  templateUrl: './my-claims.html',
  styleUrl: './my-claims.css'
})
export class MyClaims implements OnInit {
  private insuranceService = inject(InsuranceService);
  private cdr = inject(ChangeDetectorRef);

  claims: any[] = [];
  policies: any[] = [];

  selectedPolicyId: number | null = null;
  description = '';

  loading = true;
  submitting = false;
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading = true;

    forkJoin({
      claims: this.insuranceService.getMyClaims(),
      policies: this.insuranceService.getMyPolicies()
    }).subscribe({
      next: (result) => {
        this.claims = result.claims;
        this.policies = result.policies.filter(
          (policy: any) => policy.status === 1
        );

        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Unable to load claims.';
        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }

  submitClaim(): void {
    const description = this.description.trim();

    if (!this.selectedPolicyId || !description || this.submitting) {
      return;
    }

    this.submitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.insuranceService
      .createClaim(this.selectedPolicyId, description)
      .subscribe({
        next: () => {
          this.successMessage = 'Claim submitted successfully!';
          this.description = '';
          this.selectedPolicyId = null;
          this.submitting = false;
          this.loadData();
          this.cdr.markForCheck();
        },
        error: (error) => {
          console.error(error);
          this.errorMessage = 'Unable to submit claim.';
          this.submitting = false;
          this.cdr.markForCheck();
        }
      });
  }

  getStatus(status: number): string {
    switch (status) {
      case 0: return 'Submitted';
      case 1: return 'Under Review';
      case 2: return 'Approved';
      case 3: return 'Denied';
      default: return 'Unknown';
    }
  }
}
