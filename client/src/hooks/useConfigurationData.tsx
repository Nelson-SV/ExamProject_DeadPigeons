import { http } from "/src/helpers";
import { AxiosResponse } from "axios";
import toast from "react-hot-toast";
import { BalanceTopUpValue, TicketPriceDto } from "/src/Api.ts";

interface ConfigurationData {
    getTopUpValues: () => Promise<BalanceTopUpValue[]>;
    getTicketPrices: () => Promise<Record<string, TicketPriceDto>>;
}

export const useConfiguration = (): ConfigurationData => {

    const getTopUpValues = async (): Promise<BalanceTopUpValue[]> => {
        return await http.topUpPrices.configurationGetTopUpPrices()
            .then((response: AxiosResponse<BalanceTopUpValue[]>) => {
                return response.data;
            })
            .catch((e) => {
                const message = e.response?.data?.message || "An unexpected error occurred.";
                toast.error(`Error: ${message}`);
                return [];
            });
    };

    const getTicketPrices = async (): Promise<Record<string, TicketPriceDto>> => {
        return await http.ticketPrices.configurationGetTicketPrices()
            .then((response: AxiosResponse<Record<string, TicketPriceDto>>) => {
                return response.data;
            })
            .catch((e) => {
                const message = e.response?.data?.message || "An unexpected error occurred.";
                toast.error(`Error: ${message}`);
                return {};
            });
    };

    return {
        getTopUpValues,
        getTicketPrices,
    };
};