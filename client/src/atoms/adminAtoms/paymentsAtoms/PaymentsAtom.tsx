import {atom, } from "/src/atoms/imports.ts";
import {PaymentDto} from "/src/Api.ts";


export const PaymentsAtom= atom<PaymentDto[]>([]);