export interface CreateEstablishmentReq {
  name: string;
  address: string;
  networkName?: string | null;
}

export interface CreateEstablishmentResp {
  networkId: number;
  establishmentId: number;
}

export interface Network {
  id: number;
  name: string;
  establishments: Establishment[];
}

export interface Establishment {
  id: number;
  name: string;
  address: string;
  usersCount: number;
}
