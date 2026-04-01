import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Users } from "./user.model";
import { environment } from "../../../environments/environment";
import { ApiResponse } from "../../core/model/api-response";

@Injectable({ providedIn: 'root' })
export class UserService {
    private baseUrl = `${environment.baseUrl}/users`;

    constructor(private http: HttpClient) { }

    getDataWithPaging(
        page: number = 1,
        pageSize: number = 10,
        keyword: string = ''
    ) {
        const params = new HttpParams()
            .set('page', page)
            .set('pageSize', pageSize)
            .set('keyword', keyword);
        return this.http.get<ApiResponse<Users[]>>(`${this.baseUrl}`, { params });
    }

    create(data: Partial<Users>) {
        return this.http.post<ApiResponse<Users>>(this.baseUrl, data);
    }

    getById(id: string) {
        return this.http.get<ApiResponse<Users>>(`${this.baseUrl}/${id}`);
    }

    update(id: string, data: Partial<Users>) {
        return this.http.put<ApiResponse<Users>>(`${this.baseUrl}/${id}`, data);
    }

    delete(ids: string[]) {
        return this.http.post<ApiResponse<boolean>>(`${this.baseUrl}/delete`, { ids });
    }
}
