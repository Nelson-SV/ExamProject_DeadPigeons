import {atom} from "jotai";

export const InstructionsMessages = atom<string[]>([

    "Vælg det beløb, du ønsker at fylde op.",
    "Indsæt mobile pay-transaktions-id eller upload et billede med dine mobile pay-transaktionsoplysninger.",
    "Klik på knappen \"Køb\" for at behandle din betaling.",
    "Din saldo vil blive opdateret efter administrator bekræftelse af betaling."
])