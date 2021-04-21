class ApiRequest {
  constructor() {}

  public async post<TBody, TResponse>(
    endpoint: string,
    body: TBody
  ): Promise<TResponse> {
    // Default options are marked with *
    const response = await fetch(this.buildEndpoint(endpoint), {
      method: 'POST',
      mode: 'cors',
      cache: 'no-cache',
      credentials: 'same-origin',
      headers: {
        'Content-Type': 'application/json',
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

  private buildEndpoint(endpoint: string): string {
    return this.buildBaseUrl() + endpoint;
  }

  private buildBaseUrl(): string {
    return 'THE_SERVER_BASE_URL/';
  }
}

export const apiRequest = new ApiRequest();
