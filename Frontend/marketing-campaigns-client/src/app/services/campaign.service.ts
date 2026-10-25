import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Campaign } from '../models/campaign';

@Injectable({
  providedIn: 'root'
})
export class CampaignService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'https://localhost:7259/api/Campaigns';

  getCampaigns(status?: string): Observable<Campaign[]> {
    let params = new HttpParams();

    if (status) {
      params = params.set('status', status);
    }

    return this.http.get<Campaign[]>(this.apiUrl, { params });
  }

  createCampaign(campaign: {
    name: string;
    totalAmount: number;
    startDate: string;
    endDate: string;
    status: string;
  }): Observable<Campaign> {
    return this.http.post<Campaign>(this.apiUrl, campaign);
  }

  updateCampaignStatus(id: string, status: string): Observable<unknown> {
  return this.http.put(
    `${this.apiUrl}/${id}/status`,
    { status }
  );
}

consumeAmount(id: string, amount: number): Observable<unknown> {
  return this.http.post(
    `${this.apiUrl}/${id}/consume`,
    { amount }
  );
}
}