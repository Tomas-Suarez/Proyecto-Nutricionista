import axios from "axios";
import { ApiRoutes } from "../constants/ApiRoutes";

const apiClient = axios.create({
  baseURL: ApiRoutes.BASE_URL,
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
});

let isRefreshing = false;
let queue: Array<() => void> = [];

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config as any;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      if (isRefreshing) {
        return new Promise((resolve) => {
          queue.push(() => resolve(apiClient(originalRequest)));
        });
      }

      isRefreshing = true;

      try {
        await apiClient.post(ApiRoutes.Usuario.Refresh);
        queue.forEach((cb) => cb());
        queue = [];
        return apiClient(originalRequest);
      } catch {
        queue = [];
        window.location.href = "/login";
        return Promise.reject(error);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  },
);

export default apiClient;
