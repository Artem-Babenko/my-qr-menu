const key = 'token';

export const tokenStorage = {
  get: () => {
    return localStorage.getItem(key);
  },
  set: (token: string) => {
    localStorage.setItem(key, token);
  },
  remove: () => {
    localStorage.removeItem(key);
  },
};
