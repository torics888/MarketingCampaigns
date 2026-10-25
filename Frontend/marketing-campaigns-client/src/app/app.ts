import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CampaignService } from './services/campaign.service';
import { Campaign } from './models/campaign';
import { HttpErrorResponse } from '@angular/common/http';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private readonly campaignService = inject(CampaignService);
  private readonly cdr = inject(ChangeDetectorRef);
  campaigns: Campaign[] = [];
  selectedStatus = '';
  loading = false;
  saving = false;
  showCreateForm = false;
  errorMessage = '';
  createError = '';
  successMessage = '';

  newCampaign = this.getEmptyCampaign();

  ngOnInit(): void {
    this.loadCampaigns();
  }

  get minEndDate(): string {
  if (!this.newCampaign.startDate) {
    return '';
  }

  const date = new Date(`${this.newCampaign.startDate}T00:00:00Z`);
  date.setUTCDate(date.getUTCDate() + 1);
  return date.toISOString().slice(0, 10);
}

  loadCampaigns(status: string = this.selectedStatus): void {
    this.selectedStatus = status;
    this.loading = true;
    this.errorMessage = '';

    this.campaignService.getCampaigns(status || undefined).subscribe({
      next: campaigns => {
        this.campaigns = campaigns;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: error => {
        console.error('Error carregant campanyes:', error);
        this.errorMessage = 'Comprova que l’API estigui en funcionament i torna-ho a provar.';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onStatusChange(status: string): void {
    this.loadCampaigns(status);
  }

  clearFilter(): void {
    this.loadCampaigns('');
  }

  getCountByStatus(status: string): number {
    return this.campaigns.filter(
      campaign => campaign.status.toLowerCase() === status.toLowerCase()
    ).length;
  }

  getTotalBudget(): number {
    return this.campaigns.reduce(
      (total, campaign) => total + campaign.totalAmount, 0
    );
  }

  openCreateForm(): void {
    this.newCampaign = this.getEmptyCampaign();
    this.createError = '';
    this.showCreateForm = true;
  }

  closeCreateForm(): void {
    if (this.saving) {
      return;
    }

    this.showCreateForm = false;
    this.createError = '';
  }

  createCampaign(): void {
    if (
      !this.newCampaign.name.trim() ||
      this.newCampaign.totalAmount <= 0 ||
      !this.newCampaign.startDate ||
      !this.newCampaign.endDate ||
      this.newCampaign.endDate <= this.newCampaign.startDate
    ) {
      this.createError = 'Revisa les dades introduïdes abans de continuar.';
      return;
    }

    this.saving = true;
    this.createError = '';
    this.successMessage = '';

    const campaign = {
      ...this.newCampaign,
      name: this.newCampaign.name.trim()
    };

    this.campaignService.createCampaign(campaign).subscribe({
      next: () => {
        this.saving = false;
        this.showCreateForm = false;
        this.selectedStatus = '';
        this.successMessage = 'La campanya s’ha creat correctament.';
        this.loadCampaigns('');
      },
      error: error => {
        console.error('Error creant la campanya:', error);
        this.saving = false;
        this.createError =
          error.status === 0
            ? 'No s’ha pogut connectar amb l’API.'
            : error.error?.message ?? 'No s’ha pogut crear la campanya. Revisa les dades i torna-ho a provar.';
      }
    });
  }

  statusChanges: Record<string, string> = {};
consumeAmounts: Record<string, number | null> = {};
actionMessage = '';
actionError = '';
processingCampaignId: string | null = null;

updateStatus(id: string): void {
  const status = this.statusChanges[id];

  if (!status) {
    return;
  }

  this.processingCampaignId = id;
  this.actionMessage = '';
  this.actionError = '';

  this.campaignService.updateCampaignStatus(id, status).subscribe({
    next: () => {
      this.processingCampaignId = null;
      this.actionMessage = 'L’estat de la campanya s’ha actualitzat.';
      this.loadCampaigns();
    },
    error: (error: HttpErrorResponse) => {
      console.error('Error actualitzant l’estat:', error);
      this.processingCampaignId = null;
      this.actionError =
        error.error?.message ?? 'No s’ha pogut actualitzar l’estat.';
    }
  });
}

consumeBudget(id: string): void {
  const amount = this.consumeAmounts[id];

  if (amount == null || !Number.isFinite(amount) || amount <= 0) {
    this.actionError = 'Introdueix un import superior a zero.';
    return;
  }

  this.processingCampaignId = id;
  this.actionMessage = '';
  this.actionError = '';

  this.campaignService.consumeAmount(id, amount).subscribe({
    next: () => {
      this.processingCampaignId = null;
      this.consumeAmounts[id] = null;
      this.actionMessage = 'El pressupost s’ha actualitzat correctament.';
      this.loadCampaigns();
    },
    error: (error: HttpErrorResponse) => {
      console.error('Error consumint pressupost:', error);
      this.processingCampaignId = null;
      this.actionError =
        error.error?.message ?? 'No s’ha pogut consumir el pressupost.';
    }
  });
}

  private getEmptyCampaign() {
    return {
      name: '',
      totalAmount: 0,
      startDate: '',
      endDate: '',
      status: 'Esborrany'
    };
  }
}