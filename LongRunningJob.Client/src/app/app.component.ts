import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ProcessingService } from './services/processing.service';
import { Subscription } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatToolbarModule} from '@angular/material/toolbar';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, FormsModule, MatCardModule, MatInputModule, MatProgressBarModule, MatToolbarModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit, OnDestroy {
  inputText: string = '';
  outputText: string = '';
  progress: number = 0;
  maxProgress: number = 100;
  isProcessing: boolean = false;

  private subscriptions: Subscription[] = [];
  private snackBar = inject(MatSnackBar);

  constructor(private processingService: ProcessingService) {}
  ngOnInit() {
    this.subscriptions.push(
      this.processingService.onCharacterReceived().subscribe(char => {
        this.outputText += char;
        console.log('Received character: ' + char);
      })
    );

    this.subscriptions.push(
      this.processingService.onProcessingComplete().subscribe(() => {
        this.isProcessing = false;
        this.snackBar.open('Processing completed.', 'Close', {
          duration: 3000,
        });
        console.log('Processing completed');
      })
    );

    this.subscriptions.push(
      this.processingService.onProcessingCancelled().subscribe(() => {
        this.isProcessing = false;
        this.snackBar.open('Processing canceled.', 'Close', {
          duration: 3000,
        });
        console.log('Processing canceled');
      })
    );

    this.subscriptions.push(
      this.processingService.onProgress().subscribe(progress => {
        this.progress = progress;
        console.log('Progress: ' + progress);
      })
    );

    this.subscriptions.push(
      this.processingService.onProcessingCancelled().subscribe(connectionId => {
        this.isProcessing = false;
        this.progress = 0;
        console.log('Connection ID: ' + connectionId);
      })
    );
  }

  startProcessing() {
    if (this.isProcessing || !this.inputText) return;

    this.isProcessing = true;
    this.outputText = '';
    this.processingService.startProcessing(this.inputText).subscribe({
      error: (err) => {
        this.isProcessing = false;
        this.snackBar.open('Failed to start processing.', 'Close', {
          duration: 3000,
        });
      },
    });

    console.log('Processing started, input: ' + this.inputText + ', connectionId: ' + this.processingService.connectionId);
  }

  cancelProcessing() {
    if (!this.isProcessing) return;

    this.processingService.cancelProcessing().subscribe({
      error: (err) => {
        this.snackBar.open('Failed to cancel processing.', 'Close', {
          duration: 3000,
        });
      },
    });
    console.log('Processing canceled');
  }

  clearOutput() {
    window.location.reload();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  title = 'LongRunningJob.Client';
}
