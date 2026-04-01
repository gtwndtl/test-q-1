import { Component, OnInit, inject, signal, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { DatePickerModule } from 'primeng/datepicker';
import { TextareaModule } from 'primeng/textarea';

import { Users } from './user.model';
import { UserService } from './user.service';

@Component({
  selector: 'app-user',
  imports: [
    CommonModule, ReactiveFormsModule,
    TableModule, DialogModule, DatePickerModule,
    TextareaModule, ButtonModule, InputTextModule
  ],
  templateUrl: './user.html',
  styleUrl: './user.css',
  providers: []
})
export class User implements OnInit {
  private readonly userService = inject(UserService);
  private readonly destroyRef = inject(DestroyRef);

  readonly users = signal<Users[]>([]);
  readonly page = signal<number>(0);
  readonly pageSize = signal<number>(10);
  readonly totalRecords = signal<number>(0);
  readonly isLoading = signal<boolean>(false);

  isDialogVisible = false;
  isViewMode = false;

  readonly userForm = new FormGroup({
    firstname: new FormControl<string>('',
      {
        nonNullable: true,
        validators: [Validators.required]
      }
    ),
    lastname: new FormControl<string>('',
      {
        nonNullable: true,
        validators: [Validators.required]
      }
    ),
    dateOfBirth: new FormControl<Date | null>(new Date(),
      {
        validators: [Validators.required]
      }
    ),
    age: new FormControl<number>(0,
      {
        nonNullable: true,
        validators: [Validators.required, Validators.min(0)]
      }
    ),
    address: new FormControl<string>('',
      {
        nonNullable: true,
        validators: [Validators.required]
      }
    )
  });

  ngOnInit(): void {
    this.updateAge();
  }

  private updateAge(): void {
    this.userForm.controls.dateOfBirth.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((selectedDate) => this.calculateAge(selectedDate));
  }

  private calculateAge(dob: Date | null | undefined): void {
    if (!dob) {
      this.userForm.controls.age.setValue(0);
      return;
    }
    const today = new Date();
    const birthDate = new Date(dob);
    let age = today.getFullYear() - birthDate.getFullYear();
    this.userForm.controls.age.setValue(Math.max(age, 0));
  }

  fetchUsers(): void {
    this.isLoading.set(true);
    const apiPage = this.page() + 1;
    this.userService.getDataWithPaging(apiPage, this.pageSize())
      .pipe(
        finalize(() => this.isLoading.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.users.set(response.data ?? []);
            this.totalRecords.set(response['total'] ?? 0);
          }
        },
        error: (err) => {
          console.error('[UserService] fetchUsers error:', err);
        }
      });
  }

  onPageChange(event: any): void {
    const pageIndex = Math.floor((event.first ?? 0) / (event.rows ?? 10));
    this.page.set(pageIndex);
    this.pageSize.set(event.rows ?? 10);
    this.fetchUsers();
  }

  openCreateDialog(): void {
    this.isViewMode = false;
    this.userForm.enable();
    this.userForm.reset({
      firstname: '',
      lastname: '',
      dateOfBirth: new Date(),
      age: 0,
      address: ''
    });
    this.isDialogVisible = true;
  }

  openViewDialog(userId: string): void {
    this.isViewMode = true;
    this.userService.getById(userId).subscribe({
      next: (response) => {
        if (response.success) {
          this.userForm.patchValue({
            firstname: response.data?.firstname,
            lastname: response.data?.lastname,
            dateOfBirth: new Date(response.data?.dateOfBirth || ''),
            age: response.data?.age,
            address: response.data?.address
          }, { emitEvent: false });
          this.userForm.disable({ emitEvent: false });
          this.isDialogVisible = true;
        }
      },
      error: (err) => {
        console.error('[UserService] getById error:', err);
      }
    });
  }

  closeDialog(): void {
    this.isDialogVisible = false;
  }

  onSaveForm(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }

    this.userService.create(this.userForm.value as Partial<Users>).subscribe({
      next: (response) => {
        if (response.success) {
          this.fetchUsers();
          this.closeDialog();
        } else {
        }
      },
      error: (err) => {
        console.error('[UserService] create error:', err);
      }
    });
  }
}
