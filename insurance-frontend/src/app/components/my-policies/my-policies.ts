
import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InsuranceService } from '../../services/insurance';

@Component({
  selector: 'app-my-policies',
  imports: [CommonModule],
  templateUrl: './my-policies.html',
  styleUrl: './my-policies.css'
})
export class MyPolicies implements OnInit {
  private insuranceService = inject(InsuranceService);
  private cdr = inject(ChangeDetectorRef);
  policies: any[] = [];
  loading = true;
  errorMessage = '';

  ngOnInit(): void {
    this.insuranceService.getMyPolicies().subscribe({
      next: (data) => {
        this.policies = data;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Unable to load your policies.';
        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }
}

