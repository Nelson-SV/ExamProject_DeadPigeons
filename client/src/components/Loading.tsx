export const Loading = () => {
    return (
        <div className={"flex flex-row h-screen w-screen justify-center items-center mx-auto"}>
            <span className="loading loading-ring loading-xs"></span>
            <span className="loading loading-ring loading-sm"></span>
            <span className="loading loading-ring loading-md"></span>
            <span className="loading loading-ring loading-lg"></span>
        </div>
    )

}