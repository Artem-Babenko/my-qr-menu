export interface CategoryView {
  id: number;
  name: string;
  description: string | null;
  sortOrder: number;
  isActive: boolean;
  networkId: number;
}

export interface CreateCategoryRequest {
  name: string;
  description: string | null;
  sortOrder: number;
  isActive: boolean;
  networkId: number;
}

export interface UpdateCategoryRequest {
  name: string;
  description: string | null;
  sortOrder: number;
  isActive: boolean;
}

