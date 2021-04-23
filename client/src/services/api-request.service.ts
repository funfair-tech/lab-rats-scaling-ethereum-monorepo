import window from '@funfair-tech/wallet-sdk/window';

class ApiRequest {
  public async post<TBody, TResponse>(
    endpoint: string,
    body: TBody
  ): Promise<TResponse> {
    const jwt = await window.funwallet.sdk.auth.app.jwt();
    // Default options are marked with *
    const response = await fetch(this.buildEndpoint(endpoint), {
      method: 'POST',
      mode: 'cors',
      cache: 'no-cache',
      credentials: 'same-origin',
      headers: {
        'Content-Type': 'application/json',
        'x-auth-token': 'Bearer ' + jwt,
      },
      redirect: 'follow',
      referrerPolicy: 'no-referrer',
      body: JSON.stringify(body),
    });

    return (await response.json()) as TResponse;
  }

  public async get<TResponse>(endpoint: string): Promise<TResponse> {
    // Default options are marked with *
    const response = await fetch(this.buildEndpoint(endpoint), {
      method: 'GET',
      mode: 'cors',
      cache: 'no-cache',
      credentials: 'same-origin',
      headers: {
        'Content-Type': 'application/json',
      },
      redirect: 'follow',
      referrerPolicy: 'no-referrer',
    });
    return (await response.json()) as TResponse;
  }

  public buildEndpoint(endpoint: string): string {
    return this.buildBaseUrl() + endpoint;
  }

  private buildBaseUrl(): string {
    return 'https://dev.optimism.labs.funfair.io/';
  }
}

export const apiRequest = new ApiRequest();
