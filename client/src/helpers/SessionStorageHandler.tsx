interface StorageHandler {
    getItem: (key: string) => any;
    setItem: (key: string, value: any) => void;
    removeItem: (key: string) => void;
}

export const useStorageHandler: StorageHandler = {
    getItem: (key: string) => {
        const storedValue = sessionStorage.getItem(key);
        return storedValue ? JSON.parse(storedValue) : null;
    },
    setItem: (key: string, value: any) => {
        if (value) {
            sessionStorage.setItem(key, JSON.stringify(value));
        } else {
            sessionStorage.removeItem(key);
        }
    },
    removeItem: (key: string) => {
        sessionStorage.removeItem(key);
    },
};
