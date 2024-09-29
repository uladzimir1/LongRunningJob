import { Injectable } from '@angular/core';
import { Observable, from, Subject } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { UUID } from 'crypto';
import { Progress } from '../Models/progress';

@Injectable({
  providedIn: 'root',
})
export class ProcessingService {
  private hubConnection: signalR.HubConnection;
  private characterSubject = new Subject<string>();
  private progressSubject = new Subject<number>();
  private processingCompleteSubject = new Subject<void>();
  private processingCanceledSubject = new Subject<void>();

  public connectionId: string | null = null;

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5052/processing')
      .build();

    this.hubConnection.on('Character', (char: string) => {
      this.characterSubject.next(char);
      console.log('Processing service received char: ' + char);
    });

    this.hubConnection.on('Started', (jobId: UUID) => {
      console.log('Processing started. Job ID: ' + jobId);
    });

    this.hubConnection.on('Completed', (jobId: UUID) => {
      this.processingCompleteSubject.next();
      console.log('Processing completed. Job ID: ' + jobId);
    });

    this.hubConnection.on('Canceled', (jobId: UUID) => {
      this.processingCanceledSubject.next();
      console.log('Processing canceled. Job ID: ' + jobId);
    });

    this.hubConnection.on('Progress', (p: Progress) => {
      this.progressSubject.next(p.progress * 100 / p.overall);
      console.log('Progress: ' + p.progress + '/' + p.overall);
    });

    this.startConnection();
  }

  startProcessing(inputText: string): Observable<void> {
    return from(
      this.hubConnection.invoke('StartProcessing', inputText)
    );
  }
  cancelProcessing(): Observable<void> {
    return from(
      this.hubConnection.invoke('CancelProcessing')
    );
  }

  private startConnection() : void {
    this.hubConnection.start()
    .then(() => {
      console.log('Hub connection started');
      this.getConnectionId();
    })
    .catch(err => console.error(err));
  }

  private getConnectionId(): void {
    this.hubConnection.invoke('GetConnectionId')
      .then((connectionId: string) => {
        this.connectionId = connectionId;
        console.log('Connection ID: ' + connectionId);
      })
      .catch(err => console.error(err));
  }


  onCharacterReceived(): Observable<string> {
    return this.characterSubject.asObservable();
  }

  onProcessingComplete(): Observable<void> {
    return this.processingCompleteSubject.asObservable();
  }

  onProcessingCancelled(): Observable<void> {
    return this.processingCanceledSubject.asObservable();
  }

  onProgress(): Observable<number> {
    return this.progressSubject.asObservable();
  }
}
