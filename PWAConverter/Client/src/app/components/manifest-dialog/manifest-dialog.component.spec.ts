import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManifestDialogComponent } from './manifest-dialog.component';

describe('ManifestDialogComponent', () => {
  let component: ManifestDialogComponent;
  let fixture: ComponentFixture<ManifestDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManifestDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManifestDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
