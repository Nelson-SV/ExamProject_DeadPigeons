// @ts-ignore
import image from "../../resources/Untitled_Artwork (1).gif";
import React from "react";

export const GameAnimation =()=>{

    return (
        <div className=" mt-5">
            <img
                src={image}
                alt="Animated GIF"
                className="w-full h-auto max-w-md sm:max-w-lg md:max-w-md lg:max-w-lg"
            />
        </div>

    )
}