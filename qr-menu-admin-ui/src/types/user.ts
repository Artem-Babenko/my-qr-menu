export interface User {
  id: number;
  name: string;
  surname: string;
  email: string;
  phone: string;
  networkId: number | null;

  // optional: present when returned by /users/by-network/{networkId}
  accesses?: UserEstablishmentAccess[];
}

export interface UserEstablishmentAccess {
  establishmentId: number;
  roleId: number;
}

export interface UpdateUserEstablishmentsRequest {
  accesses: UserEstablishmentAccess[];
}
