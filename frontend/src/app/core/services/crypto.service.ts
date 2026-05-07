import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CryptoService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiUrl;
  private cachedKey: CryptoKey | null = null;

  private async fetchKey(): Promise<CryptoKey> {
    if (this.cachedKey) return this.cachedKey;

    const res = await firstValueFrom(
      this.http.get<{ data: { publicKey: string } }>(`${this.base}/auth/public-key`)
    );

    const binary = Uint8Array.from(atob(res.data.publicKey), c => c.charCodeAt(0));

    this.cachedKey = await crypto.subtle.importKey(
      'spki',
      binary,
      { name: 'RSA-OAEP', hash: 'SHA-256' },
      false,
      ['encrypt']
    );

    return this.cachedKey;
  }

  async encrypt(plaintext: string): Promise<string> {
    const key = await this.fetchKey();
    const encoded = new TextEncoder().encode(plaintext);
    const encrypted = await crypto.subtle.encrypt({ name: 'RSA-OAEP' }, key, encoded);
    return btoa(String.fromCharCode(...new Uint8Array(encrypted)));
  }
}
