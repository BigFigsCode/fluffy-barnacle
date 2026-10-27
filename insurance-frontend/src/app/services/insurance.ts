
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface InsuranceProduct {
  productId: number;
  name: string;
  description: string;
  basePremium: number;
  isActive: boolean;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class InsuranceService {
  private http = inject(HttpClient);
  private apiUrl = 'https://insurance-policy-management-webapi-cudchfg2dpfzdkag.australiaeast-01.azurewebsites.net/api';

  getProducts() {
    return this.http.get<InsuranceProduct[]>(
      `${this.apiUrl}/InsuranceProduct`
    );
  }

  login(credentials: LoginRequest) {
    return this.http.post<LoginResponse>(
      `${this.apiUrl}/Auth/login`,
      credentials
    );
  }

  getMyPolicies() {
    return this.http.get<any[]>(`${this.apiUrl}/Policy/MyPolicies`);
  }

  createPolicy(productId: number) {
    return this.http.post<{ message: string; policyId: number }>(
      `${this.apiUrl}/Policy`,
      { productId }
    );
  }


  getMyClaims() {
    return this.http.get<any[]>(`${this.apiUrl}/Claim/MyClaims`);
  }

  createClaim(policyId: number, description: string) {
    return this.http.post<{ message: string; claimId: number }>(
      `${this.apiUrl}/Claim`,
      { policyId, description }
    );
  }

  getAllPolicies() {
    return this.http.get<any[]>(`${this.apiUrl}/Policy/AllPolicies`);
  }

  activatePolicy(policyId: number) {
    return this.http.patch<{ message: string }>(
      `${this.apiUrl}/Policy/${policyId}/activate`,
      {}
    );
  }

  getAllClaims() {
    return this.http.get<any[]>(`${this.apiUrl}/Claim/AllClaims`);
  }

  updateClaimStatus(claimId: number, status: number) {
    return this.http.patch<void>(
      `${this.apiUrl}/Claim/${claimId}/status`,
      { status }
    );
  }

  deactivateProduct(productId: number) {
    return this.http.patch<void>(
      `${this.apiUrl}/InsuranceProduct/${productId}/deactivate`,
      {}
    );
  }

  activateProduct(productId: number) {
    return this.http.patch<void>(
      `${this.apiUrl}/InsuranceProduct/${productId}/activate`,
      {}
    );
  }

    register(user: {
      firstName: string;
      lastName: string;
      email: string;
      password: string;
    }) {
      return this.http.post<{ message: string; userId: number }>(
        `${this.apiUrl}/Auth/Register`,
        user
      );
    }
  }
