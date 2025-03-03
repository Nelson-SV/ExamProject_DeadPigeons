/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export interface UsersDetailsPageResultDto {
  items: UsersDetailsDto[] | null;
  /** @format int32 */
  totalItems: number;
  /** @format int32 */
  page: number;
  /** @format int32 */
  pageSize: number;
}

export interface UsersDetailsDto {
  userId: string;
  name: string | null;
  email: string | null;
  phoneNumber: string | null;
  isActive: boolean | null;
}

export interface LoginResponse {
  jwt: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  name: string;
  role: string;
  phoneNumber: string;
}

export interface InitPasswordResetRequest {
  email: string;
}

export interface UpdateUserDto {
  userId: string;
  name: string | null;
  email: string | null;
  phoneNumber: string | null;
  isActive: boolean | null;
}

export interface AuthUserInfo {
  username: string;
  isAdmin: boolean;
  canPlay: boolean;
  /** @format int32 */
  balanceValue: number;
  userId: string;
}

export interface ResetPasswordResponse {
  success: boolean;
  message: string | null;
}

export interface ResetPasswordDto {
  password: string | null;
  userId: string | null;
}

export interface PasswordResetRequest {
  email: string;
  token: string;
  password: string;
}

export interface UpdatedBalanceResponse {
  name: string | null;
  mediaLink: string | null;
  /** @format int32 */
  currentBalanceValue: number;
}

export type ValueTupleOfCurrentBalanceDtoAndListOfAutomatedTicketsDto = object;

export interface CreateTicketDto {
  /** @format date-time */
  purchaseDate: string;
  userId: string;
  gameId: string;
  sequence: number[];
  /** @format int32 */
  priceValue: number;
  isAutomated: boolean;
}

export interface AutomatedTicketsDto {
  /** @format guid */
  guid: string;
  /** @format date-time */
  purchaseDate: string;
  sequence: number[];
  userId: string;
  /** @format int32 */
  priceValue: number;
  isActive: boolean;
}

export interface GameIdDto {
  guid: string;
}

export interface UpdateAutomatedTicketStatusRequest {
  automatedTicket: AutomatedTicketsDto;
  isActive: boolean;
}

export interface GameTicketsPageResultDto {
  items: GameTicketDetailedDto[];
  /** @format int32 */
  totalItems: number;
  /** @format int32 */
  page: number;
  /** @format int32 */
  pageSize: number;
}

export interface GameTicketDetailedDto {
  /** @format guid */
  guid: string;
  ticketNumber: string;
  /** @format date-time */
  purchaseDate: string;
  formattedPurchaseDate: string;
  sequence: number[];
  userId: string;
  /** @format int32 */
  priceValue: number;
  extractedNumbers: number[] | null;
  /** @format decimal */
  winnings: number | null;
  gameId: string;
}

export interface TicketPriceDto {
  /** @format date-time */
  created: string;
  /** @format date-time */
  updated: string | null;
  /** @format int32 */
  numberOfFields: number;
  /** @format int32 */
  price: number;
}

export interface PaymentPageResultDtoOfPaymentDto {
  items: PaymentDto[];
  /** @format int32 */
  totalItems: number;
  /** @format int32 */
  page: number;
  /** @format int32 */
  pageSize: number;
}

export interface PaymentDto {
  guid: string;
  name: string;
  bucket: string;
  /** @format date-time */
  timeCreated: string;
  mediaLink: string;
  transactionId: string;
  /** @format int32 */
  value: number;
  userId: string;
  userName: string;
}

export interface UpdatePaymentDto {
  guid: string;
  /** @format int32 */
  value: number;
  userId: string;
  /** @format date-time */
  updated: string;
}

export interface DeclinePaymentDto {
  guid: string;
  name: string;
  userId: string;
  bucket: string;
  userName: string;
}

export interface CurrentGameDto {
  guid: string;
  /** @format date-time */
  startDate: string;
  formattedStartDate: string;
  /** @format date-time */
  extractionDate: string | null;
  extractedNumbers: number[] | null;
  /** @format int32 */
  revenue: number | null;
  /** @format int32 */
  rolloverValue: number | null;
  status: boolean;
}

export interface ProcessedWinningSequence {
  registered: boolean;
  message: string;
  uninsertedTickets: InsufficientFundDto[];
}

export interface InsufficientFundDto {
  userName: string;
  userEmail: string | null;
  /** @format int32 */
  balanceValue: number;
  failedToInsertTickets: GameTicketDto[];
  /** @format date-time */
  currentGameDate: string;
}

export interface GameTicketDto {
  /** @format date-time */
  purchaseDate: string;
  sequence: number[];
  userId: string;
  /** @format int32 */
  priceValue: number;
}

export interface WinningSequenceDto {
  winningSequence: number[] | null;
  gameId: string | null;
}

export interface WinningPlayersDto {
  winningPlayers: WinningPlayer[] | null;
  pagination: Pagination | null;
}

export interface WinningPlayer {
  playerId: string;
  userName: string | null;
  email: string | null;
  /** @format int32 */
  winningCount: number;
  winningTicketsIds: string[];
}

export interface Pagination {
  /** @format int32 */
  currentPageItems: number;
  /** @format int32 */
  currentPage: number;
  /** @format int32 */
  nextPage: number | null;
  hasNext: boolean;
  /** @format int32 */
  totalItems: number;
}

export interface WinningPlayersRequestDto {
  gameId: GameIdDto | null;
  pagination: Pagination | null;
  winningSequence: number[] | null;
}

export interface TupleOfListOfCurrentGameDtoAndPagination {
  item1: CurrentGameDto[] | null;
  item2: Pagination | null;
}

export interface PlayerTicketsResponseDto {
  playerTickets: TicketsResponseDto[] | null;
}

export interface TicketsResponseDto {
  /** @format guid */
  guid: string;
  /** @format date-time */
  purchaseDate: string;
  formattedPurchaseDate: string;
  /** @format int32 */
  priceValue: number;
  playedNumbers: number[];
}

export interface PlayerTicketsRequestDto {
  playerTicketsIds: string[] | null;
}

export interface GetGameRequestDto {
  /** @format int32 */
  week: number;
  /** @format int32 */
  year: number;
}

export interface PaymentPageResultDtoOfPaymentDto {
  items: PaymentDto[];
  /** @format int32 */
  totalItems: number;
  /** @format int32 */
  page: number;
  /** @format int32 */
  pageSize: number;
}

export interface PaymentDto {
  guid: string;
  name: string;
  /** @format date-time */
  timeCreated: string;
  /** @format date-time */
  updated: string;
  mediaLink: string;
  transactionId: string;
  /** @format int32 */
  value: number;
  userId: string;
}

export interface UpdatePaymentDto {
  guid: string;
  /** @format int32 */
  value: number;
  userId: string;
}

export interface DeclinePaymentDto {
  guid: string;
  name: string;
  userId: string;
}

import type { AxiosInstance, AxiosRequestConfig, AxiosResponse, HeadersDefaults, ResponseType } from "axios";
import axios from "axios";

export type QueryParamsType = Record<string | number, any>;

export interface FullRequestParams extends Omit<AxiosRequestConfig, "data" | "params" | "url" | "responseType"> {
  /** set parameter to `true` for call `securityWorker` for this request */
  secure?: boolean;
  /** request path */
  path: string;
  /** content type of request body */
  type?: ContentType;
  /** query params */
  query?: QueryParamsType;
  /** format of response (i.e. response.json() -> format: "json") */
  format?: ResponseType;
  /** request body */
  body?: unknown;
}

export type RequestParams = Omit<FullRequestParams, "body" | "method" | "query" | "path">;

export interface ApiConfig<SecurityDataType = unknown> extends Omit<AxiosRequestConfig, "data" | "cancelToken"> {
  securityWorker?: (
    securityData: SecurityDataType | null,
  ) => Promise<AxiosRequestConfig | void> | AxiosRequestConfig | void;
  secure?: boolean;
  format?: ResponseType;
}

export enum ContentType {
  Json = "application/json",
  FormData = "multipart/form-data",
  UrlEncoded = "application/x-www-form-urlencoded",
  Text = "text/plain",
}

export class HttpClient<SecurityDataType = unknown> {
  public instance: AxiosInstance;
  private securityData: SecurityDataType | null = null;
  private securityWorker?: ApiConfig<SecurityDataType>["securityWorker"];
  private secure?: boolean;
  private format?: ResponseType;

  constructor({ securityWorker, secure, format, ...axiosConfig }: ApiConfig<SecurityDataType> = {}) {
    this.instance = axios.create({ ...axiosConfig, baseURL: axiosConfig.baseURL || "http://localhost:5000" });
    this.secure = secure;
    this.format = format;
    this.securityWorker = securityWorker;
  }

  public setSecurityData = (data: SecurityDataType | null) => {
    this.securityData = data;
  };

  protected mergeRequestParams(params1: AxiosRequestConfig, params2?: AxiosRequestConfig): AxiosRequestConfig {
    const method = params1.method || (params2 && params2.method);

    return {
      ...this.instance.defaults,
      ...params1,
      ...(params2 || {}),
      headers: {
        ...((method && this.instance.defaults.headers[method.toLowerCase() as keyof HeadersDefaults]) || {}),
        ...(params1.headers || {}),
        ...((params2 && params2.headers) || {}),
      },
    };
  }

  protected stringifyFormItem(formItem: unknown) {
    if (typeof formItem === "object" && formItem !== null) {
      return JSON.stringify(formItem);
    } else {
      return `${formItem}`;
    }
  }

  protected createFormData(input: Record<string, unknown>): FormData {
    if (input instanceof FormData) {
      return input;
    }
    return Object.keys(input || {}).reduce((formData, key) => {
      const property = input[key];
      const propertyContent: any[] = property instanceof Array ? property : [property];

      for (const formItem of propertyContent) {
        const isFileType = formItem instanceof Blob || formItem instanceof File;
        formData.append(key, isFileType ? formItem : this.stringifyFormItem(formItem));
      }

      return formData;
    }, new FormData());
  }

  public request = async <T = any, _E = any>({
    secure,
    path,
    type,
    query,
    format,
    body,
    ...params
  }: FullRequestParams): Promise<AxiosResponse<T>> => {
    const secureParams =
      ((typeof secure === "boolean" ? secure : this.secure) &&
        this.securityWorker &&
        (await this.securityWorker(this.securityData))) ||
      {};
    const requestParams = this.mergeRequestParams(params, secureParams);
    const responseFormat = format || this.format || undefined;

    if (type === ContentType.FormData && body && body !== null && typeof body === "object") {
      body = this.createFormData(body as Record<string, unknown>);
    }

    if (type === ContentType.Text && body && body !== null && typeof body !== "string") {
      body = JSON.stringify(body);
    }

    return this.instance.request({
      ...requestParams,
      headers: {
        ...(requestParams.headers || {}),
        ...(type ? { "Content-Type": type } : {}),
      },
      params: query,
      responseType: responseFormat,
      data: body,
      url: path,
    });
  };
}

/**
 * @title My Title
 * @version 1.0.0
 * @baseUrl http://localhost:5000
 */
export class Api<SecurityDataType extends unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @name Get
   * @request GET:/
   * @secure
   */
  get = (params: RequestParams = {}) =>
    this.request<string, any>({
      path: `/`,
      method: "GET",
      secure: true,
      format: "json",
      ...params,
    });

  api = {
    /**
     * No description
     *
     * @tags AdminUserManagement
     * @name AdminUserManagementGetUsersDetails
     * @request GET:/api/admin/users-management
     * @secure
     */
    adminUserManagementGetUsersDetails: (
      query?: {
        adminId?: string;
        /**
         * @format int32
         * @default 1
         */
        page?: number;
        /**
         * @format int32
         * @default 5
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.request<UsersDetailsPageResultDto, any>({
        path: `/api/admin/users-management`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags AdminUserManagement
     * @name AdminUserManagementDeleteUser
     * @request DELETE:/api/admin/delete-user
     * @secure
     */
    adminUserManagementDeleteUser: (
      query?: {
        userId?: string;
      },
      params: RequestParams = {},
    ) =>
      this.request<File, any>({
        path: `/api/admin/delete-user`,
        method: "DELETE",
        query: query,
        secure: true,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Auth
     * @name AuthLogin
     * @request POST:/api/auth/login
     * @secure
     */
    authLogin: (data: LoginRequest, params: RequestParams = {}) =>
      this.request<LoginResponse, any>({
        path: `/api/auth/login`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Auth
     * @name AuthRegister
     * @request POST:/api/auth/create-user
     * @secure
     */
    authRegister: (data: RegisterRequest, params: RequestParams = {}) =>
      this.request<UsersDetailsDto, any>({
        path: `/api/auth/create-user`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Auth
     * @name AuthInitPasswordReset
     * @request POST:/api/auth/init-password-reset
     * @secure
     */
    authInitPasswordReset: (data: InitPasswordResetRequest, params: RequestParams = {}) =>
      this.request<File, any>({
        path: `/api/auth/register`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Auth
     * @name AuthUpdateUser
     * @request PUT:/api/auth/update-user
     * @secure
     */
    authUpdateUser: (data: UpdateUserDto, params: RequestParams = {}) =>
      this.request<UsersDetailsDto, any>({
        path: `/api/auth/update-user`,
        method: "PUT",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Auth
     * @name AuthLogout
     * @request POST:/api/auth/logout
     * @secure
     */
    authLogout: (params: RequestParams = {}) =>
      this.request<File, any>({
        path: `/api/auth/logout`,
        method: "POST",
        secure: true,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Auth
     * @name AuthUserInfo
     * @request GET:/api/auth/userinfo
     * @secure
     */
    authUserInfo: (params: RequestParams = {}) =>
      this.request<AuthUserInfo, any>({
        path: `/api/auth/userinfo`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Auth
     * @name AuthResetPassword
     * @request POST:/api/auth/reset
     * @secure
     */
    authResetPassword: (data: ResetPasswordDto, params: RequestParams = {}) =>
      this.request<ResetPasswordResponse, any>({
        path: `/api/auth/reset`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Auth
     * @name AuthPasswordReset
     * @request POST:/api/auth/password-reset
     * @secure
     */
    authPasswordReset: (data: PasswordResetRequest, params: RequestParams = {}) =>
      this.request<File, any>({
        path: `/api/auth/password-reset`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Play
     * @name PlayCreateGameTicket
     * @request POST:/api/play/gameTickets
     * @secure
     */
    playCreateGameTicket: (data: CreateTicketDto[], params: RequestParams = {}) =>
      this.request<ValueTupleOfCurrentBalanceDtoAndListOfAutomatedTicketsDto, any>({
        path: `/api/play/gameTickets`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Play
     * @name PlayGetAutomatedTickets
     * @request GET:/api/play
     * @secure
     */
    playGetAutomatedTickets: (
      query?: {
        userId?: string;
      },
      params: RequestParams = {},
    ) =>
      this.request<AutomatedTicketsDto[], any>({
        path: `/api/play`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Play
     * @name PlayCheckIsAllowedToPlay
     * @request GET:/api/play/checkIsAllowedToPlay
     * @secure
     */
    playCheckIsAllowedToPlay: (params: RequestParams = {}) =>
      this.request<GameIdDto, any>({
        path: `/api/play/checkIsAllowedToPlay`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Play
     * @name PlayDeleteAutomatedTicket
     * @request DELETE:/api/play/deleteAutomatedTicket
     * @secure
     */
    playDeleteAutomatedTicket: (data: AutomatedTicketsDto, params: RequestParams = {}) =>
      this.request<File, any>({
        path: `/api/play/deleteAutomatedTicket`,
        method: "DELETE",
        body: data,
        secure: true,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Play
     * @name PlayUpdateAutomatedTicketStatus
     * @request PUT:/api/play/updateAutomatedTicketStatus
     * @secure
     */
    playUpdateAutomatedTicketStatus: (data: UpdateAutomatedTicketStatusRequest, params: RequestParams = {}) =>
      this.request<File, any>({
        path: `/api/play/updateAutomatedTicketStatus`,
        method: "PUT",
        body: data,
        secure: true,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name UserGetUserTicketsHistory
     * @request GET:/api/user/history
     * @secure
     */
    userGetUserTicketsHistory: (
      query?: {
        userId?: string;
        /**
         * @format int32
         * @default 1
         */
        page?: number;
        /**
         * @format int32
         * @default 5
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.request<GameTicketsPageResultDto, any>({
        path: `/api/user/history`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags GameHandler
     * @name GameHandlerGetCurrentGamInfo
     * @request GET:/api/admin/currentGame
     * @secure
     */
    gameHandlerGetCurrentGamInfo: (params: RequestParams = {}) =>
      this.request<CurrentGameDto, any>({
        path: `/api/admin/currentGame`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags GameHandler
     * @name GameHandlerSetWinningNumbers
     * @request POST:/api/admin/settWinningSequence
     * @secure
     */
    gameHandlerSetWinningNumbers: (data: WinningSequenceDto, params: RequestParams = {}) =>
      this.request<ProcessedWinningSequence, any>({
        path: `/api/admin/settWinningSequence`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags GameHandler
     * @name GameHandlerGetWinningPlayers
     * @request POST:/api/admin/getWinningPlayers
     * @secure
     */
    gameHandlerGetWinningPlayers: (data: WinningPlayersRequestDto, params: RequestParams = {}) =>
      this.request<WinningPlayersDto, any>({
        path: `/api/admin/getWinningPlayers`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags GameHandler
     * @name GameHandlerGetGameById
     * @request POST:/api/admin/getGameById
     * @secure
     */
    gameHandlerGetGameById: (data: GameIdDto, params: RequestParams = {}) =>
      this.request<CurrentGameDto, any>({
        path: `/api/admin/getGameById`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags GameHandler
     * @name GameHandlerGetGames
     * @request POST:/api/admin/getGames
     * @secure
     */
    gameHandlerGetGames: (data: Pagination, params: RequestParams = {}) =>
      this.request<TupleOfListOfCurrentGameDtoAndPagination, any>({
        path: `/api/admin/getGames`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),


    /**
     * No description
     *
     * @tags GameHandler
     * @name GameHandlerGetWinningTicketsForPlayer
     * @request POST:/api/admin/getWinningTicketsForPlayer
     * @secure
     */
    gameHandlerGetWinningTicketsForPlayer: (data: PlayerTicketsRequestDto, params: RequestParams = {}) =>
      this.request<PlayerTicketsResponseDto, any>({
        path: `/api/admin/getWinningTicketsForPlayer`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags GameHandler
     * @name GameHandlerGetGameByWeekAndYear
     * @request POST:/api/admin/getGameByWeekAndYear
     * @secure
     */
    gameHandlerGetGameByWeekAndYear: (data: GetGameRequestDto, params: RequestParams = {}) =>
      this.request<CurrentGameDto[], any>({
        path: `/api/admin/getGameByWeekAndYear`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags GameHandler
     * @name GameHandlerGetGameByWeekAndYear
     * @request POST:/api/admin/getGameByWeekAndYear
     * @secure
     */
    paymentGetUserPendingPayments: (
      query?: {
        /**
         * @format int32
         * @default 1
         */
        page?: number;
        /**
         * @format int32
         * @default 5
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.request<PaymentPageResultDtoOfPaymentDto, any>({
        path: `/api/adminPayment/pendingPayments`,
        method: "GET",
        query: query,
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Payment
     * @name PaymentUpdatePendingPayments
     * @request POST:/api/adminPayment/updatePendingPayments
     * @secure
     */
    paymentUpdatePendingPayments: (data: UpdatePaymentDto, params: RequestParams = {}) =>
      this.request<File, any>({
        path: `/api/adminPayment/updatePendingPayments`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Payment
     * @name PaymentDeclinePendingPayments
     * @request POST:/api/adminPayment/declinePendingPayments
     * @secure
     */
    paymentDeclinePendingPayments: (data: DeclinePaymentDto, params: RequestParams = {}) =>
      this.request<File, any>({
        path: `/api/adminPayment/declinePendingPayments`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        ...params,
      }),
  };
  user = {
    /**
     * No description
     *
     * @tags Payment
     * @name PaymentUploadImage
     * @request POST:/user/balance
     * @secure
     */
    paymentUploadImage: (
      data: {
        ContentType?: string | null;
        ContentDisposition?: string | null;
        Headers?: any[] | null;
        /** @format int64 */
        Length?: number;
        Name?: string | null;
        FileName?: string | null;
        /** @format int32 */
        topUpValue?: number;
        authUserId?: string | null;
        transactionId?: string | null;
      },
      params: RequestParams = {},
    ) =>
      this.request<UpdatedBalanceResponse, any>({
        path: `/user/balance`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.FormData,
        format: "json",
        ...params,
      }),
  };
  ticketPrices = {
    /**
     * No description
     *
     * @tags Configuration
     * @name ConfigurationGetTicketPrices
     * @request GET:/ticketPrices
     * @secure
     */
    configurationGetTicketPrices: (params: RequestParams = {}) =>
      this.request<Record<string, TicketPriceDto>, any>({
        path: `/ticketPrices`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),
  };
  topUpPrices = {
    /**
     * No description
     *
     * @tags Configuration
     * @name ConfigurationGetTopUpPrices
     * @request GET:/topUpPrices
     * @secure
     */
    configurationGetTopUpPrices: (params: RequestParams = {}) =>
      this.request<number[], any>({
        path: `/topUpPrices`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),
  };
}
