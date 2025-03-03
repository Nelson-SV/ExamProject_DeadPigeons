import {Api} from '../Api.ts';
import { tokenStorage } from "/src/hooks/imports.ts";
import {StorageKeys} from "/src/helpers/index.ts";

export const baseUrl = import.meta.env.VITE_APP_BASE_API_URL;

// URL prefix for own server
// This is to protect us from accidently sending the JWT to 3rd party services.
const AUTHORIZE_ORIGIN = "/";

const _api = new Api({baseURL:baseUrl});

_api.instance.interceptors.request.use((config) => {
    // Get the JWT from storage.
    const jwt = tokenStorage.getItem(StorageKeys.TOKEN_KEY, null);
    // Add Authorization header if we have a JWT and the request goes to our own
    // server.
    if (jwt && config.url?.startsWith(AUTHORIZE_ORIGIN)) {
        // Set Authorization header, so server can tell hos is logged in.
        config.headers.Authorization = `Bearer ${jwt}`;
    }
    return config;
});

// Expose API-client which will handle authorization.
export const http = _api;