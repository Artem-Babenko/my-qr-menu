export interface TableView {
  id: number;
  number: string;
  establishmentId: number;
}

export interface CreateTableRequest {
  establishmentId: number;
  number: string;
}

export interface UpdateTableRequest {
  number: string;
}
