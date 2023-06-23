import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourceCollectorDialogComponent } from './resource-collector-dialog.component';

describe('ResourceCollectorDialogComponent', () => {
  let component: ResourceCollectorDialogComponent;
  let fixture: ComponentFixture<ResourceCollectorDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ResourceCollectorDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ResourceCollectorDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
