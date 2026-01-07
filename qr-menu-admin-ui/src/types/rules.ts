export type ValidationRule<T = unknown> = (val?: T | null) => true | string;
