import Pagination from "/src/components/shared/Pagination.tsx";
import ManageUsersModal from "./ManageUsersModal";
import React, {useState} from "react";
import {useAtom} from "jotai";
import { http } from "/src/helpers";
import toast from "react-hot-toast";
import {UsersDetailsAtom, useInitializeUsersDetails, PaginationAtom} from "/src/components/imports.ts";
import ConfirmationWindowModal from "./ConfirmationWindowModal.tsx";
import {UsersDetailsDto} from "/src/Api.ts";

export default function UserManagementPage() {

    const [sortConfig, setSortConfig] = useState({ key: null, direction: "default" });
    const [usersDetails, setUsersDetails] = useAtom(UsersDetailsAtom);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages] = useAtom(PaginationAtom);
    const [isModalOpen, setModalOpen] = useState(false);
    const [modalMode, setModalMode] = useState("");
    const [selectedUser, setSelectedUser] = useState<UsersDetailsDto | null>(null);
    const [confirmDeleteModal, setConfirmDeleteModal] = useState(false);

    useInitializeUsersDetails({page: currentPage});

    const sortedData = React.useMemo(() => {
        if (!sortConfig.key) return usersDetails;

        const sorted = [...usersDetails].sort((a, b) => {
            if (!sortConfig.key) return 0;

            const key = sortConfig.key;

            if (a[key] < b[key]) return sortConfig.direction === "asc" ? -1 : 1;
            if (a[key] > b[key]) return sortConfig.direction === "asc" ? 1 : -1;

            return 0;
        });

        return sorted;
    }, [usersDetails, sortConfig]);

    const handleSort = (key) => {
        setSortConfig((prevConfig) => {
            if (prevConfig.key === key) {
                const nextDirection = prevConfig.direction === "asc" ? "desc" : prevConfig.direction === "desc" ? "default" : "asc";
                return { key: nextDirection === "default" ? null : key, direction: nextDirection };
            }
            return { key, direction: "asc" };
        });
    };

    const openCreateModal = () => {
        setModalMode("create");
        setSelectedUser(null);
        setModalOpen(true);
    };

    const openEditModal = (user) => {
        setModalMode("edit");
        setSelectedUser(user);
        setModalOpen(true);
    };

    const openDeleteModal = (user) => {
        setSelectedUser(user);
        setConfirmDeleteModal(true);
    };

    const handleSaveUser = (userData) => {
        try {
            if (modalMode === "create") {
                http.api.authRegister(userData).then(r => {
                    if(r.status === 200){
                        setUsersDetails((prevUsers) => [...prevUsers, r.data]);
                        toast.success("User added successfully.");
                        http.api.authInitPasswordReset({email: userData.email}).then(r => {
                            if(r.status === 200){
                                toast.success(`Email send to ${userData.name} successfully.`);
                            }
                        });
                    } else {
                        toast.error(`Failed to send email to ${userData.name}. 
                        Please delete this account and create a new one.`);
                    }
                });
            } else {
                http.api.authUpdateUser(userData).then(r => {
                    if(r.status === 200){
                        setUsersDetails((prevUsers) =>
                            prevUsers.map((user) => (user.userId === r.data.userId ? r.data : user))
                    );
                    toast.success("User updated successfully.");
                    } else {
                        toast.error("Unexpected error occurred.");
                    }
                });
            }
            setModalOpen(false);
        } catch (error) {
            toast.error("Error performing operation for the user: " + error);
        }
    };

    const handleDeleteUser = async () => {
        try {
            if(selectedUser != null) {
                if(selectedUser.userId != null){
                    const response = await http.api.adminUserManagementDeleteUser({ userId: selectedUser.userId });
                    if (response.status === 200) {
                        setUsersDetails((prevUsers) =>
                            prevUsers.filter((user) => user.userId !== selectedUser.userId)
                        );
                        toast.success("User deleted successfully.");
                    } else {
                        toast.error("Error deleting user.");
                    }
                }
                else{
                    toast.error("An unexpected error occurred, please try again later.");
                }
            }
        } catch (error) {
            toast.error("An unexpected error occurred: " + error);
        } finally {
            setConfirmDeleteModal(false);
            setSelectedUser(null);
        }
    };

    const handleCancelDelete = () => {
        setConfirmDeleteModal(false);
        setSelectedUser(null);
    };

    return (
        <div className="flex justify-center">
            <div className="overflow-x-auto px-4 sm:px-32 py-10">
                <div className="flex justify-end items-center mb-4 w-full">
                    <button
                        className="w-36 h-12 bg-red-600 text-white py-2 font-semibold rounded-md border border-transparent hover:bg-black hover:text-white hover:border-black transition-colors duration-300 text-center"
                        onClick={openCreateModal}
                    >
                        Tilføj ny bruger
                    </button>
                </div>
                <table className="table-auto w-full border-separate border-spacing-y-2">
                    <thead className="hidden sm:table-header-group">
                    <tr>
                        <th
                            className="w-52 text-base cursor-pointer text-black text-left pl-4"
                            onClick={() => handleSort("name")}
                        >
                            NAVN
                        </th>
                        <th
                            className="w-72 text-base cursor-pointer text-black text-left pl-4"
                            onClick={() => handleSort("email")}
                        >
                            EMAIL
                        </th>
                        <th className="w-52 text-base text-black text-left pl-4">MOBILTELEFON</th>
                        <th className="w-36 text-base text-black text-left pl-4">STATUS</th>
                    </tr>
                    </thead>
                    <tbody>
                    {Array.isArray(sortedData) && sortedData.length > 0 ? (
                        sortedData.map((userDetail, index) => (
                            <tr
                                key={userDetail.userId || index}
                                className="hover:shadow-md rounded-3xl bg-background flex sm:table-row flex-wrap sm:flex-nowrap mb-4 sm:mb-0"
                            >
                                <td className="p-2 sm:p-4 font-bold rounded-s-2xl w-full sm:w-auto text-left">
                                    <span className="sm:hidden text-gray-500 block">Navn:</span>
                                    {userDetail.name}
                                </td>
                                <td className="p-2 sm:p-4 font-bold w-full sm:w-auto text-left">
                                    <span className="sm:hidden text-gray-500 block">Email:</span>
                                    {userDetail.email}
                                </td>
                                <td className="p-2 sm:p-4 font-bold w-full sm:w-auto text-left">
                                    <span className="sm:hidden text-gray-500 block">Mobiltelefon:</span>
                                    {userDetail.phoneNumber}
                                </td>
                                <td className="p-2 sm:p-4 font-bold w-full sm:w-auto text-left">
                                    <span className="sm:hidden text-gray-500 block">Status:</span>
                                    {userDetail.isActive ? (
                                        <div className="w-20 badge badge-success text-white">Aktiv</div>
                                    ) : (
                                        <div className="w-20 badge badge-error bg-red-500 text-white">Inaktiv</div>
                                    )}
                                </td>
                                <td className="p-2 sm:p-4 font-bold rounded-e-2xl w-full sm:w-auto text-left whitespace-nowrap">
                                    <button
                                        className="ml-2 sm:ml-10 px-3 py-1 bg-blue-400 text-white rounded-md"
                                        onClick={() => openEditModal(userDetail)}
                                    >
                                        Rediger
                                    </button>
                                    <button
                                        className="ml-2 sm:ml-5 px-3 py-1 bg-red-500 text-white rounded-md"
                                        onClick={() => openDeleteModal(userDetail)}
                                    >
                                        Sletning
                                    </button>
                                </td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan={5} className="text-center pt-20 text-xl sm:text-4xl">
                                Der er i øjeblikket ingen brugere
                            </td>
                        </tr>
                    )}
                    </tbody>
                </table>
                <Pagination
                    totalPages={totalPages}
                    currentPage={currentPage}
                    onPageChange={setCurrentPage}
                />
                <ManageUsersModal
                    isOpen={isModalOpen}
                    onClose={() => setModalOpen(false)}
                    onSave={handleSaveUser}
                    mode={modalMode}
                    user={selectedUser}
                />
                <ConfirmationWindowModal
                    isOpen={confirmDeleteModal}
                    title="Bekræft sletning"
                    message={`Er du sikker på, at du vil slette ${selectedUser?.name}? Denne handling kan ikke fortrydes.`}
                    onConfirm={handleDeleteUser}
                    onCancel={handleCancelDelete}
                />
            </div>
        </div>
    );


}