import { Routes } from '@angular/router';
import { MainLayout } from './main-layout/main-layout';
import { Welcome } from './pages/welcome/welcome';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { DashboardLayout } from './pages/dashboard/dashboard-layout/dashboard-layout';
import { authGuard } from './route-guards/auth-guard';
import { SchemaLayout } from './pages/schema/schema-layout/schema-layout';

export const routes: Routes = [
    {
        path: '',
        component: MainLayout, //Template Layout
        children: [
            {
                //Child Components to be dynamically filled in the mainlayout component
                path: '',
                component: Welcome
            },
            {
                path: 'login',
                component: Login
            },
            {
                path: 'register',
                component: Register
            },
            
        ]

    },
    {
        //Temporary for development with no guard
        path: 'schema',
        component: SchemaLayout
    },
    {
        path:'dashboard',
        component: DashboardLayout,
        canActivate: [authGuard]
    },
];
