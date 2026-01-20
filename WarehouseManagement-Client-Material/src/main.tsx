import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'
import {BrowserRouter} from "react-router";
import { AllCommunityModule, ModuleRegistry } from 'ag-grid-community';
import {QueryClient,QueryClientProvider} from "@tanstack/react-query";

ModuleRegistry.registerModules([AllCommunityModule]);
const client=new QueryClient();
createRoot(document.getElementById('root')!).render(
    <QueryClientProvider client={client}>
        <BrowserRouter>
            <App />
        </BrowserRouter>
    </QueryClientProvider>
)
