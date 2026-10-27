
import {
  Component,
  inject,
  OnInit,
  ChangeDetectorRef
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { InsuranceService, InsuranceProduct } from '../../services/insurance';
import { Router } from '@angular/router';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products implements OnInit {
  private insuranceService = inject(InsuranceService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  products: InsuranceProduct[] = [];
  loading = true;
  errorMessage = '';

  purchasingProductId: number | null = null;
  purchaseError = '';

  ngOnInit(): void {
    this.insuranceService.getProducts().subscribe({
      next: (data) => {
        // Only display products available to customers
        this.products = data.filter(product => product.isActive);

        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Unable to load insurance products.';
        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }

  purchasePolicy(productId: number): void {
    if (this.purchasingProductId !== null) return;

    this.purchaseError = '';
    this.purchasingProductId = productId;

    this.insuranceService.createPolicy(productId).subscribe({
      next: (response) => {
        console.log(response.message);
        this.purchasingProductId = null;
        this.router.navigate(['/my-policies']);
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error(error);

        this.purchaseError =
          error.status === 401
            ? 'Please log in to request coverage.'
            : 'Unable to create your policy. Please try again.';

        this.purchasingProductId = null;
        this.cdr.markForCheck();
      }
    });
  }
}
