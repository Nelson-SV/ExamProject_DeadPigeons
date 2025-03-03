import React, {useEffect, useState} from "react";
import {http, showErrorToasts} from "/src/helpers"
import toast from "react-hot-toast";
import {CurrentGameDto, GetGameRequestDto} from "/src/Api.ts";
import {AxiosResponse} from "axios";
import {SearchResponseGameDisplay} from "/src/components/AdminComponents/GameHistory/SearchResponseGameDisplay.tsx";
interface SearchResponse{
    week:string,
    year:string,
    weekRequired: boolean,
    yearRequired: boolean,
}
export const SearchWindowComponent = () => {
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const [searchState, setSearchState] = useState<SearchResponse>({
        week:"",
        year:"",
        weekRequired: false,
        yearRequired: false,
    });
    const [resultState, setResultState] = useState<CurrentGameDto[]>([]);
    const [displayResults, setDisplayResults] = useState<boolean>(false);
    useEffect(() => {

    }, []);

    const handleChange = (id: string, value: string|null) => {
        switch (id) {
            case "week":
                handleWeekChange(value);
                break;
            case "year":
                handleYearChange(value);
                break;
        }
    }

    const handleWeekChange = (value: string|null) => {
        setSearchState((prev) => (
            {...prev, week: value ? value : "", weekRequired: true}))
    }

    const handleYearChange = (value: string|null) => {
        setSearchState((prev) => (
            {...prev, year: value?value:"", yearRequired: true}))
    }
    const switchOpen = async (e) => {
        e.stopPropagation()
        if (isOpen) {
            setSearchState({
                week: "",
                year: "",
                weekRequired: false,
                yearRequired: false,

            })
            setResultState([])
            setDisplayResults(false)
        }
        setIsOpen(!isOpen);
    }

    const retrieveGames = (e) => {
        e.preventDefault();

        if (!isValidYearInput(searchState.year!)) {
            setSearchState((prev) => ({...prev, yearRequired: true}))
            return;
        }
        if (!isValidWeekInput(searchState.week!)) {
            setSearchState((prev) => ({...prev, weekRequired: true}))
            return;
        }
        const searchRequest: GetGameRequestDto = {
            week: Number.parseInt(searchState.week!),
            year: Number.parseInt(searchState.year!)
        }

        http.api.gameHandlerGetGameByWeekAndYear(searchRequest)
            .then((r: AxiosResponse<CurrentGameDto[]>) => {
                if (r.status === 200) {
                    setResultState(r.data);
                    setDisplayResults(true);
                }
            })
            .catch((e) => {
                setDisplayResults(false);
                const errorData = e.response?.data;
                if (errorData?.errors) {
                    showErrorToasts(errorData);
                } else {
                    toast.error("Network error. Please try again later.");
                }
            })
    }
    const isValidWeekInput = (value: string): boolean => {
        const num = Number.parseInt(value);
        return !isNaN(num) && num >= 1 && num <= 52;
    }

    const isValidYearInput = (value: string): boolean => {
        const isFourDigits = value.length === 4 && /^\d{4}$/.test(value);
        if (!isFourDigits) return false;
        let number = Number.parseInt(value);
        return number >= 0;
    }


    return (
        <>
            <div
                className={`absolute top-16 right-2 ${isOpen ? "collapse-open" : "collapse-close"}  rounded-sm z-50 w-1/2 sm:w-1/2 md:w-1/3 lg:w-1/3 xl:w-1/4  max-h-[80vh] overflow-hidden`}>
                <div className=" text-xl font-medium flex flex-row justify-center items-center ">
                    <button className=" input input-bordered flex justify-center items-center gap-2 w-1/2 mt-1"
                            onClick={(e) => switchOpen(e)}>
                        <p className="grow">Søge</p>
                        <svg
                            xmlns="http://www.w3.org/2000/svg"
                            viewBox="0 0 16 16"
                            fill="currentColor"
                            className="h-4 w-4 opacity-70"
                        >
                            <path
                                fillRule="evenodd"
                                d="M9.965 11.026a5 5 0 1 1 1.06-1.06l2.755 2.754a.75.75 0 1 1-1.06 1.06l-2.755-2.754ZM10.5 7a3.5 3.5 0 1 1-7 0 3.5 3.5 0 0 1 7 0Z"
                                clipRule="evenodd"
                            />
                        </svg>
                    </button>
                </div>
                <div className="collapse-content mt-2 bg-hover rounded-sm">
                    <form className="flex flex-col gap-2 mb-2">
                        <div className="flex flex-col">
                            <label htmlFor="year" className="font-bold mb-1 text-text">Year:</label>
                            <input
                                type="number"
                                id="year"
                                name="year"
                                min="2024"
                                max="2100"
                                placeholder="e.g., 2024"
                                value={searchState.year}
                                onChange={(e) => handleChange(e.target.id, e.target.value||null)}
                                required={searchState.yearRequired}
                                className={"peer invalid:[&:not(:placeholder-shown):not(:focus)]:border-red-500 p-2 text-sm border-2 border-blue-300 rounded-md outline-none "}
                            />
                            <p className={"text-transparent peer-invalid:text-red-500 peer-focus-within:text-transparent "}>Please
                                enter a valid value. E.g., 2024 </p>
                        </div>

                        <div className="flex flex-col">
                            <label htmlFor="week" className="font-bold mb-1 text-text">Week:</label>
                            <input
                                className={"peer invalid:[&:not(:placeholder-shown):not(:focus)]:border-red-500  p-2 text-sm border-2 border-blue-300 rounded-md outline-none "}
                                onChange={(e) => handleChange(e.target.id, e.target.value||null)}
                                type="number"
                                id="week"
                                name="week"
                                min="1"
                                max="53"
                                placeholder="e.g., 12"
                                value={searchState.week}
                                required={searchState.weekRequired}
                            />
                            <p className={"text-transparent peer-invalid:text-red-500 peer-focus-within:text-transparent"}>Please
                                enter a valid value between 1 and 52. E.g., 12 </p>
                        </div>
                        <button onClick={(e) => {
                            retrieveGames(e)
                        }}
                                className="w-36 h-15 text-white py-2 bg-black font-semibold rounded-md border border-transparent hover:bg-red-500 hover:text-white hover:border-black transition-colors duration-300 text-center">
                            Søge
                        </button>
                    </form>
                    {displayResults &&
                        (resultState.length > 0 ? (
                            <div className="h-32 overflow-y-scroll flex flex-col justify-start items-stretch gap-2 ">
                                {resultState.map((r, i) => (
                                    <SearchResponseGameDisplay
                                        key={r.guid}
                                        currentGame={r}
                                        index={i}
                                    />
                                ))}
                            </div>
                        ) : (
                            <div className=" bg-border text-hover_text flex items-center justify-center">
                                <p>No results</p>
                            </div>
                        ))
                    }
                </div>

            </div>
        </>
    )
}