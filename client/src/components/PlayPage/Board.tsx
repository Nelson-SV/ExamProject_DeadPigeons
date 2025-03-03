import React, {useEffect, useState} from "react";
import '../../styles/Board.css'
// @ts-ignore
import image2 from "../../resources/close.png";
import {useAtom} from "jotai";
import {Boards} from "../imports.ts";
import {useConfiguration} from "/src/hooks";
import {TicketPriceDto} from "/src/Api.ts";
import toast from "react-hot-toast";


const Board: React.FC<{boardIndex: number }> = ({boardIndex}) => {
    const [boards, setBoards] = useAtom(Boards);
    const {getTicketPrices} = useConfiguration();
    const [ticketPrices, setTicketPrices] = useState<Record<string, TicketPriceDto>>({});

    useEffect(() => {
        const fetchPrices = async () => {
            try {
                const prices = await getTicketPrices();
                setTicketPrices(prices);
            } catch (err: any) {
                toast.error(err.message);
            }
        };

        fetchPrices();
    }, []);


    const handleClick = (index: number) => {
        /* Toggle the clicked state of the specific button and also update the sequence*/
        setBoards((prevBoards) => {
            const updatedBoards = [...prevBoards];
            const currentBoard = updatedBoards[boardIndex];

            /* Toggle the button state*/
            const isSelected = !currentBoard.buttons[index];
            currentBoard.buttons[index] = isSelected;

            /* If the button is selected, add the 1-based index to the sequence (index + 1)*/
            if (isSelected) {
                currentBoard.sequence.push(index + 1);
            } else {

                currentBoard.sequence = currentBoard.sequence.filter((seqIndex) => seqIndex !== index + 1);
            }

            currentBoard.price = calculatePrice(currentBoard.sequence.length)

            return updatedBoards;
        });
    };

    const calculatePrice = (numFields: number): number| null => {
        /* Uses the numFields directly as a string key*/
        const priceKey = `${numFields}`;
        const priceEntry = ticketPrices[priceKey];

        if (!priceEntry) {
            return null;
        }

        return priceEntry.price;
    };


    const displayPrice = (price: number | null) => {
        if (price === null) {
            return "Vælg 5,6,7 eller 8";
        }
        return `${price} DKK`;
    };


    return (
        <div className="flex justify-center items-center p-4 ">
            <div
                className="flex flex-col items-center gap-5  border-4 rounded-2xl border-[#272626] bg-[#dddddd]  sm:w-[350px] sm:h-[410px] md:w-[400px] md:h-[450px] lg:w-[350px] lg:h-[410px] resize overflow-auto relative"
                style={{
                    minWidth: '250px', // Set a minimum width for resizing
                    minHeight: '300px', // Set a minimum height for resizing
                }}
            >
                {/* Header Section */}
                <div
                    className="w-[170px] h-[38px] bg-white rounded-3xl mt-5 text-[20px] font-semibold text-center text-[#272626]">
                    Døde Duer
                    <div className="price-display text-[16px]">
                        <p>{displayPrice(boards[boardIndex].price)}</p>
                    </div>
                </div>

                {/* Resizable Board Container */}
                <div
                    className="grid grid-cols-4 grid-rows-4 gap-[15px] p-[29px] w-[85%] h-[75%]  mb-4 bg-[#c5c5c5] border-[3px] border-[#272525] box-border
                    "
                >
                    {boards[boardIndex].buttons.map((clicked, index) => {
                        const buttonId = `${index + 1}`; // Assign a unique ID based on the button's number (1-based)

                        return (
                            <button
                                key={buttonId}
                                id={buttonId}
                                className="board-button bg-white border-3 border-[#272525] text-[#272525] font-bold flex justify-center items-center aspect-square cursor-pointer relative"
                                onClick={() => handleClick(index)}
                                style={{
                                    fontSize: '20px',
                                }}
                            >
                                <div className="button-content flex justify-center items-center w-full h-full relative">
                                    <span className="button-number absolute z-10 text-[1em]">{index + 1}</span>
                                    <img
                                        src={image2}
                                        alt="X"
                                        className={`x-image absolute z-20 transition-opacity duration-200 ${clicked ? 'opacity-100' : 'opacity-0'}`}
                                        style={{
                                            width: '30px', // Image size for buttons
                                            height: '30px', // Image size for buttons
                                        }}
                                    />
                                </div>
                            </button>
                        );
                    })}
                </div>
            </div>
        </div>


    );
}
export default Board;
