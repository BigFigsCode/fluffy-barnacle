import { Routes } from '@angular/router';
import { Products } from './components/products/products';
import { Login } from './components/login/login';
import { MyPolicies } from './components/my-policies/my-policies';
import { MyClaims } from './components/my-claims/my-claims';
import { AdminDashboard } from './components/admin-dashboard/admin-dashboard';
import { authGuard } from './guards/auth-guard';
import { adminGuard } from './guards/admin-guard';
import { Register } from './components/register/register';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'products', component: Products, canActivate: [authGuard] },
  { path: 'my-policies', component: MyPolicies, canActivate: [authGuard] },
  { path: 'admin', component: AdminDashboard, canActivate: [adminGuard] },
  { path: 'my-claims', component: MyClaims, canActivate: [authGuard] },
  { path: 'register', component: Register },
  { path: '**', redirectTo: 'login' },
];
