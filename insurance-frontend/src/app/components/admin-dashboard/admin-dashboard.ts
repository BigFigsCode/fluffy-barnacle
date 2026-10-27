
import {
  Component,
  ChangeDetectorRef,
  OnInit,
  inject
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { forkJoin } from 'rxjs';
import { InsuranceProduct, InsuranceService } from '../../services/insurance';

@Component({
  selector: 'app-admin-dashboard',
  imports: [CommonModule],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboard implements OnInit {
  private insuranceService = inject(InsuranceService);
  private cdr = inject(ChangeDetectorRef);

  policies: any[] = [];
  claims: any[] = [];
  products: InsuranceProduct[] = [];

  loading = true;
  processing = false;
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading = true;

    forkJoin({
      policies: this.insuranceService.getAllPolicies(),
      claims: this.insuranceService.getAllClaims(),
      products: this.insuranceService.getProducts()
    }).subscribe({
      next: (data) => {
        this.policies = data.policies;
        this.claims = data.claims;
        this.products = data.products;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Unable to load admin data.';
        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }

  activatePolicy(policyId: number): void {
    if (this.processing) return;

    this.processing = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.insuranceService.activatePolicy(policyId).subscribe({
      next: () => {
        this.successMessage = `Policy #${policyId} activated successfully.`;
        this.processing = false;
        this.loadData();
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Unable to activate policy.';
        this.processing = false;
        this.cdr.markForCheck();
      }
    });
  }

  updateClaimStatus(claimId: number, status: number): void {
    if (this.processing) return;

    this.processing = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.insuranceService.updateClaimStatus(claimId, status).subscribe({
      next: () => {
        this.successMessage = `Claim #${claimId} updated successfully.`;
        this.processing = false;
        this.loadData();
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Unable to update claim.';
        this.processing = false;
        this.cdr.markForCheck();
      }
    });
  }

  deactivateProduct(productId: number): void {
    if (this.processing) return;

    this.processing = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.insuranceService.deactivateProduct(productId).subscribe({
      next: () => {
        this.processing = false;
        this.successMessage = `Product #${productId} deactivated successfully.`;
        this.loadData();
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error(error);
        this.processing = false;
        this.errorMessage = 'Unable to deactivate product.';
        this.cdr.markForCheck();
      }
    });
  }

  getPolicyStatus(status: number): string {
    switch (status) {
      case 0: return 'Pending';
      case 1: return 'Active';
      case 2: return 'Expired';
      case 3: return 'Cancelled';
      default: return 'Unknown';
    }
  }

  getClaimStatus(status: number): string {
    switch (status) {
      case 0: return 'Submitted';
      case 1: return 'Under Review';
      case 2: return 'Approved';
      case 3: return 'Denied';
      default: return 'Unknown';
    }
  }

  get pendingPoliciesCount(): number {
    return this.policies.filter(policy => policy.status === 0).length;
  }


  activateProduct(productId: number): void {
    if (this.processing) return;

    this.processing = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.insuranceService.activateProduct(productId).subscribe({
      next: () => {
        this.processing = false;
        this.successMessage = `Product #${productId} reactivated successfully.`;
        this.loadData();
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error(error);
        this.processing = false;
        this.errorMessage = 'Unable to reactivate product.';
        this.cdr.markForCheck();
      }
    });
  }

}
