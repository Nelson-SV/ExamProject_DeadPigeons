import { useAtom } from "jotai";
import { AddedBoards, Boards, WeeklyBoards } from "../../atoms/index.ts";
import { useInitializeData, useState, BalanceAtom } from "../imports.ts";
// @ts-ignore
import image from "/src/resources/pigeon.gif";
import { PlusCircleIcon } from "@heroicons/react/24/solid";
import { XCircleIcon } from "@heroicons/react/24/solid";
import {
    CreateTicketDto,
    ValueTupleOfCurrentBalanceDtoAndListOfAutomatedTicketsDto,
    AutomatedTicketsDto,
    UpdateAutomatedTicketStatusRequest
} from "/src/Api.ts";
import { getWeekNumber, http } from "/src/helpers";
import toast from "react-hot-toast";
import {AxiosError, AxiosResponse} from "axios";
import Board from "./Board.tsx";
import React from "react";
import {getUserInfoFromToken} from "/src/helpers/index.ts"

const PlayPage = () => {
    const [boards, setBoards] = useAtom(Boards);
    const [addedBoards, setAddedBoards] = useAtom(AddedBoards);
    const [isChecked, setIsChecked] = useState(false);
    const [weeklyBoards, setWeeklyBoards] = useAtom(WeeklyBoards);
    const [balance, setBalance] = useAtom(BalanceAtom);

    useInitializeData();


    /* Adds a new game board to the list of added boards. Checks if the first board has a valid price,
    creates a new CreateTicketDto object with the current board’s details, and adds it to the state.
    It then resets the current board and unchecks the "automation" checkbox. */
    const handleAddBoard = () => {

        if (boards[0].price !== null) {
            /* Create a new CreateTicketDto object with the required properties*/
            const newTicketDto: CreateTicketDto = {
                userId: getUserInfoFromToken().userId,
                gameId: "",
                sequence: [...boards[0].sequence],
                priceValue: boards[0].price,
                isAutomated: isChecked,
                purchaseDate: "",
            };

            /* Add the new CreateTicketDto to the AddedBoardsAtom */
            setAddedBoards((prevAddedBoards) => [...prevAddedBoards, newTicketDto]);
            resetCurrentBoard();
            setIsChecked(false);
        }
    };

    /* Resets the first board to its default state. */
    const resetCurrentBoard = () => {
        setBoards((prevBoards) => {
            /* Make a copy of the boards array*/
            const updatedBoards = [...prevBoards];
            updatedBoards[0] = {
                buttons: new Array(16).fill(false),
                sequence: [],
                price: 0,
            };
            return updatedBoards;
        });
    };

    /* Validates the user’s ability to play, updates the board data with the game ID and purchase date.
     On success, it updates the balance, adds new automated tickets to weekly boards, and clears the added boards. */
    const handlePay = async () => {
        try {
            /* Check if the user is allowed to play and return gameId*/
            const response = await http.api.playCheckIsAllowedToPlay();
            const gameId = response.data.guid;

            if (!gameId) {
                toast.error("Not possible to play, try again later");
                return;
            }

            const boardData = addedBoards.map((board) =>
                ({
                    ...board,
                    gameId: gameId,
                    purchaseDate: new Date().toISOString(),
                }));

            /* Create the game tickets and return automated tickets and balance */
            const createTicketResponse: AxiosResponse <ValueTupleOfCurrentBalanceDtoAndListOfAutomatedTicketsDto> = await http.api.playCreateGameTicket(boardData);

            if (createTicketResponse.data!=null && createTicketResponse.status === 200) {
                toast.success(`${getUserInfoFromToken().username}, your game tickets were created successfully!`);
                const updatedBalance= createTicketResponse.data.currentBalance;
                const automatedTickets = createTicketResponse.data.automatedTickets;

                setBalance(updatedBalance)
                setWeeklyBoards((prevWeeklyBoards) => [
                    ...prevWeeklyBoards,       /* Keep existing tickets*/
                    ...automatedTickets /* Append new tickets*/
                ]);

                setAddedBoards([]);

            }

        } catch (e) {
            if (e instanceof AxiosError) {
                const errorData = e.response?.data;
                if (errorData) {
                    toast.error(errorData);
                } else {
                    toast.error("An unexpected error occurred. Please try again later.");
                }
            } else {
                toast.error("Network error. Please try again later.");
            }
        }


    };


    const handleCheckboxChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setIsChecked(e.target.checked);
    };

    /* Removes a specific board from the list of added boards, filters out the board at the specified index, removing it from the state. */
    const handleRemove = (boardToRemove) => {
        setAddedBoards((prevAddedBoards) =>
            prevAddedBoards.filter((_, index) => index !== boardToRemove)
        );
    };

    const handleDelete = async (ticket: AutomatedTicketsDto) => {
        try {
            const response = await http.api.playDeleteAutomatedTicket(ticket);
            const updatedWeeklyBoards = await http.api.playGetAutomatedTickets({ userId: getUserInfoFromToken().userId });
            setWeeklyBoards(updatedWeeklyBoards.data);
            toast.success(response.data.message);
        } catch (error) {
            toast.error("An unexpected error occurred. Please try again later.");
        }
    };

    const handleToggleActive = async (ticket: AutomatedTicketsDto, isActive: boolean) => {
        try {
            const updatedStatus: UpdateAutomatedTicketStatusRequest = {
                isActive: isActive,
                automatedTicket: ticket
            }
            const response = await http.api.playUpdateAutomatedTicketStatus(updatedStatus);
            const updatedWeeklyBoards = await http.api.playGetAutomatedTickets({  userId: getUserInfoFromToken().userId });
            setWeeklyBoards(updatedWeeklyBoards.data);
            toast.success(response.data.message);
        } catch (error) {
            toast.error("An unexpected error occurred. Please try again later.");
        }
    };

    return(

        <div className='flex flex-col items-center gap-7 justify-center mt-6 '>
            <h1 className="text-2xl  leading-loose font-semibold text-transparent bg-clip-text bg-gradient-to-r from-gray-500 to-red-500">
                Uge {getWeekNumber(new Date())}
            </h1>
            <div>
                <div className="flex flex-row justify-between gap-5">
                    <img src={image} alt="Animated GIF"
                         className="w-16 h-14 sm:w-18 sm:h-16 md:w-20 md:h-18 lg:w-24 lg:h-20 object-contain"
                    />
                    <button className="flex-shrink-0 mt-6 " onClick={handleAddBoard}>
                        <PlusCircleIcon className="w-10 h-10 text-red-600  hover:text-black" />
                    </button>
                </div>
                {/* Render all the boards dynamically */}
                {boards.map((_, index) => (
                    <Board key={index} boardIndex={index} />
                ))}

                <div className="form-control items-center">
                    <label className="label cursor-pointer">
                        <span className="label-text mr-2">Spil hver uge</span>
                        <input
                            type="checkbox"
                            checked={isChecked}
                            onChange={handleCheckboxChange}
                            className="checkbox border-black [--chkbg:theme(colors.black)] [--chkfg:white] checked:border-indigo-800"
                        />
                    </label>
                </div>
            </div>

            <div>
                <button
                    onClick={handlePay}
                    className="w-36 h-15 bg-black text-white py-2 bg-red-600 font-semibold rounded-md border border-transparent hover:bg-black hover:text-white hover:border-black transition-colors duration-300 text-center"
                >
                    Købe
                </button>
            </div>

            {/* Carousel with added boards */}
                {addedBoards.length > 0 && (
                    <div className="carousel carousel-center rounded-box gap-5 p-2 space-x-4 max-w-full md:max-w-[800px]">

                    {addedBoards.map((board, index) => (
                            <div key={index}>
                                <button onClick={() => handleRemove(index)} className="ml-36">
                                    <XCircleIcon className="w-8 h-8 text-red-600 hover:text-black" />
                                </button>
                                <div className="flex-shrink-0 w-[170px] h-[160px] border-2 border-[#272626] bg-[#dddddd] rounded-xl shadow-lg flex items-center justify-center relative">
                                    {/* Board Content */}
                                    <div className="text-center bg-white w-[138px] h-[128px] border-2 border-[#272626] rounded-xl">
                                        <h3 className="font-semibold text-lg">Board {index + 1}</h3>
                                        <p>Pris: DKK{board.priceValue}</p>
                                        <p>Rækkefølge: {board.sequence.join(", ")}</p>
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>

                )}

            {/* Carousel with weekly boards */}
            {weeklyBoards.length > 0 && (
                <div className="carousel carousel-center rounded-box gap-5 p-2 space-x-4 max-w-full md:max-w-[800px]">
                    {weeklyBoards.map((board, index) => (
                        <div key={board.guid} className="relative">
                            {/* XCircleIcon Button Positioned Above Each Board */}
                            <button
                                key={`remove-board-${board.guid}`}
                                className="ml-36"
                                onClick={() => handleDelete(board)}
                            >
                                <XCircleIcon className="w-8 h-8 text-red-600 hover:text-black" />
                            </button>

                            {/* Board Content */}
                            <div className="flex-shrink-0 w-[170px] h-[200px] border-2 border-[#272626] bg-[#dddddd] rounded-xl shadow-lg flex items-center justify-center relative">
                                <div
                                    className="text-center bg-white w-[138px] h-[128px] border-2 border-[#272626] rounded-xl">
                                    <h3 className="font-semibold text-lg">Board {index + 1}</h3>
                                    <p className="justify-center">Automatiseret</p>
                                    <p>Pris: DKK{board.priceValue}</p>
                                    <p>Rækkefølge: {board.sequence.join(", ")}</p>
                                    <label className="label cursor-pointer">
                                        <span className="label-text font-bold">Er aktiv</span>
                                        <input
                                            type="checkbox"
                                            className="toggle toggle-error focus:ring-2 focus:ring-red-500"
                                            defaultChecked={board.isActive}
                                            onChange={(e) => {
                                                handleToggleActive(board, e.target.checked);
                                                e.target.blur();
                                            }}
                                        />
                                    </label>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
};

export default PlayPage;
