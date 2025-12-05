export interface LoginReq {
  phone: string;
  password: string;
}

export interface LoginResp {
  token: string;
}

export interface RegistrationReq {
  name: string;
  surname: string;
  phone: string;
  email: string;
  password: string;
}

export interface RegistrationResp {
  userId: number;
}
