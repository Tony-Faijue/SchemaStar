import { Routes } from '@angular/router';
import { MainLayout } from './main-layout/main-layout';
import { Welcome } from './pages/welcome/welcome';

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
            
        ]

    },

];
